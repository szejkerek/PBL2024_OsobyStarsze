using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using UnityEngine;

namespace Plugins.KinectModule.Data{

    /// <summary>
    /// Klasa komunkująca się z urządzeniem Azure Kinect.
    /// </summary>
    public class KinectConnection : MonoBehaviour{

        /// <summary>
        /// Id urządzenia.
        /// </summary>
        [SerializeField]
        private int id;

        /// <summary>
        /// Tryb działania urządzenia.
        /// </summary>
        [SerializeField]
        private FPS cameraFPS = FPS.FPS30;

        /// <summary>
        /// Tryb pobierania obrazu.
        /// </summary>
        [SerializeField]
        private KinectImageSettings imageSettings = KinectImageSettings.Outline;

        /// <summary>
        /// Tryb głebi.
        /// </summary>
        private DepthMode depthMode = DepthMode.NFOV_Unbinned;
        /// <summary>
        /// Format odbieranego obrazu z urządzenia.
        /// </summary>
        private ImageFormat imageFormat = ImageFormat.ColorBGRA32;
        /// <summary>
        /// Rozdzielczość odbieranego obrazu z urządzenia.
        /// </summary>
        private ColorResolution colorResolution = ColorResolution.R720p;
        /// <summary>
        /// Tryb połączenia się z urządzeniem.
        /// </summary>
        private WiredSyncMode wiredSyncMode = WiredSyncMode.Standalone;

        /// <summary>
        /// Obiekt przechowujący dane z urządzenia.
        /// </summary>
        private BackgroundData frameBackgroundData = new BackgroundData();
        /// <summary>
        /// Czy zostały poprawnie pobrane najnowsze dane.
        /// </summary>
        private bool latest;
        /// <summary>
        /// Obiekt służący do synchronizacji wątków.
        /// </summary>
        private object lockObj = new object();
        /// <summary>
        /// Przechowuje informacje czy jest to pierwsza pbrana ramka danych.
        /// </summary>
        private bool readFirstFrame;
        /// <summary>
        /// Przechowuje czas pierwszej odebranej ramki danych.
        /// </summary>
        private TimeSpan initialTimestamp;

        /// <summary>
        /// Przechowuje token, który służy do przerwania wątku.
        /// </summary>
        private CancellationTokenSource cancellationTokenSource;
        /// <summary>
        /// Przechowuje token, który służy do przerwania wątku.
        /// </summary>
        private CancellationToken token;

        /// <summary>
        /// Informacja o statusie urządzenia.
        /// </summary>
        public KinectStatus status { get; set; } = KinectStatus.Off;

        /// <summary>
        /// Przy niszczeniu obiektu rozłącza się z urządzeniem Azure Kinect.
        /// </summary>
        private void OnDestroy() {
            Dispose();   
        }

        /// <summary>
        /// Przy wyjściu z aplikacji rozłącza się z urządzeniem Azure Kinect.
        /// </summary>
        private void OnApplicationQuit() {
            Dispose();
        }

        /// <summary>
        /// Pobiera aktualną ramkę danych.
        /// </summary>
        /// <param name="dataBuffer">Referencja w której są przechowywane dane z urządzenia.</param>
        /// <returns></returns>
        public bool GetCurrentFrameData(ref BackgroundData dataBuffer) {
            lock (lockObj) {
                var temp = dataBuffer;
                dataBuffer = frameBackgroundData;
                frameBackgroundData = temp;
                bool result = latest;
                latest = false;
                return result;
            }
        }

        /// <summary>
        /// Metoda przerywająca nowy wątek.
        /// </summary>
        public void Dispose() {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }

        /// <summary>
        /// Uruchamia nowy wątek.
        /// </summary>
        public void StartThread() {
            if (status == KinectStatus.Off || status == KinectStatus.OffWithError) {
                cancellationTokenSource = new CancellationTokenSource();
                token = cancellationTokenSource.Token;
                Task.Run(() => RunBackgroundThreadAsync(id, token, new DeviceConfiguration() {
                    CameraFPS = cameraFPS,
                    ColorResolution = colorResolution,
                    DepthMode = depthMode,
                    WiredSyncMode = wiredSyncMode,
                    ColorFormat = imageFormat
                }));
            }
        }

        /// <summary>
        /// Metoda służąca do aktualizacji ramki danych z urządzenia.
        /// </summary>
        /// <param name="currentFrameData">Referencja, w której są przechowywane dane z urządzenia.</param>
        private void SetCurrentFrameData(ref BackgroundData currentFrameData) {
            lock (lockObj) {
                var temp = currentFrameData;
                currentFrameData = frameBackgroundData;
                frameBackgroundData = temp;
                latest = true;
            }
        }

        /// <summary>
        /// Metoda zwracająca odpowiednią metodę, generująca obraz z urządzenia, zależnej od zmiennej imageSettings.
        /// </summary>
        /// <returns>Metoda typu GetKinectData</returns>
        private GetKinectData GetCurrentMethod() {
            switch (imageSettings) {
                case KinectImageSettings.Outline:
                    return GetOutlineImage;
                case KinectImageSettings.RGB:
                    return GetRGBAImage;
                case KinectImageSettings.OutlineRGB:
                    return GetRGBAOutlineImage;
                default:
                    return null; 
            }
        }

        /// <summary>
        /// Metoda generuje obraz z zarysem wykrytej postaci z urządzenia. 
        /// </summary>
        /// <param name="data">Referencja, w której są przechowywane dane z urządzenia.</param>
        /// <param name="frame">Ramka danych z urządzenia.</param>
        private void GetOutlineImage(ref BackgroundData data, Frame frame) {
            
            Image colorImage = frame.BodyIndexMap;

            Span<byte> colorFrame = colorImage.Memory.Span;

            data.ColorImageHeight = colorImage.HeightPixels;
            data.ColorImageWidth = colorImage.WidthPixels;
            data.ColorImageSize = data.ColorImageHeight * data.ColorImageWidth * 4;

            if (!readFirstFrame) {
                readFirstFrame = true;
                initialTimestamp = colorImage.DeviceTimestamp;
                if (data.ColorImage.Length != data.ColorImageSize) {
                    data.ColorImage = new byte[data.ColorImageSize];
                }
            }

            data.TimestampInMs = (float)(colorImage.DeviceTimestamp - initialTimestamp).TotalMilliseconds;

            int byteCounter = 0;
            for (int it = (int)(colorFrame.Length) - 1; it >= 0 && byteCounter < data.ColorImageSize; it--) {
                data.ColorImage[byteCounter++] = 255; // B
                data.ColorImage[byteCounter++] = 255; // R
                data.ColorImage[byteCounter++] = 255; // G
                data.ColorImage[byteCounter++] = (byte)(255 - colorFrame[it]);
            }
        }

        /// <summary>
        /// Metoda generuje obraz z kamery RGB z zarysem wykrytej przez urządzenie postaci. 
        /// </summary>
        /// <param name="currentFrameData">Referencja, w której są przechowywane dane z urządzenia.</param>
        /// <param name="frame">Ramka danych z urządzenia.</param>
        private void GetRGBAOutlineImage(ref BackgroundData currentFrameData, Frame frame) {

            Capture bodyFrameCapture = frame.Capture;

            Image colorImage = bodyFrameCapture.Color;
            Image frameBody = frame.BodyIndexMap;

            currentFrameData.ColorImageHeight = colorImage.HeightPixels;
            currentFrameData.ColorImageWidth = colorImage.WidthPixels;
            currentFrameData.ColorImageSize = currentFrameData.ColorImageHeight * currentFrameData.ColorImageWidth * 4;

            if (!readFirstFrame) {
                readFirstFrame = true;
                initialTimestamp = colorImage.DeviceTimestamp;
                if (currentFrameData.ColorImage.Length != currentFrameData.ColorImageSize) {
                    currentFrameData.ColorImage = new byte[currentFrameData.ColorImageSize];
                }
            }
            currentFrameData.TimestampInMs = (float)(colorImage.DeviceTimestamp - initialTimestamp).TotalMilliseconds;

            Span<ushort> colorFrame = MemoryMarshal.Cast<byte, ushort>(colorImage.Memory.Span);
            Span<ushort> bodyFrame = MemoryMarshal.Cast<byte, ushort>(frameBody.Memory.Span);

            int bodyFrameStride = frameBody.WidthPixels / 2; 
            int byteCounter = 0;

            float ratioHeight = (float)frameBody.HeightPixels / currentFrameData.ColorImageHeight;
            float ratioWidth= (float)frameBody.WidthPixels / currentFrameData.ColorImageWidth * currentFrameData.ColorImageHeight / frameBody.HeightPixels;

            int startOutlineWidth = (int)((currentFrameData.ColorImageWidth - frameBody.WidthPixels) * (ratioHeight)) / 4;

            for (int y = currentFrameData.ColorImageHeight - 1; y >= 0; y--) {
                int itY = y * currentFrameData.ColorImageWidth;
                int itYBody = (int)(y * ratioHeight) * bodyFrameStride;
            
                for (int x = currentFrameData.ColorImageWidth - 1; x >= currentFrameData.ColorImageWidth - 1 - startOutlineWidth; x--) {
    
                    currentFrameData.ColorImage[byteCounter++] = 0; // B
                    currentFrameData.ColorImage[byteCounter++] = 0; // R
                    currentFrameData.ColorImage[byteCounter++] = 0; // G
                    currentFrameData.ColorImage[byteCounter++] = 0; // A
                }

                for (int x = currentFrameData.ColorImageWidth - 2 - startOutlineWidth; x >= startOutlineWidth; x--) {
                    int it = x + itY;
                    it *= 2;
                    it--;

                    if (it >= colorFrame.Length || it < 1) {
                        currentFrameData.ColorImage[byteCounter++] = 0; // B
                        currentFrameData.ColorImage[byteCounter++] = 0; // R
                        currentFrameData.ColorImage[byteCounter++] = 0; // G
                        currentFrameData.ColorImage[byteCounter++] = 0; // A
                        continue;
                    }

                    byte alpha = 0;
                    int itBodyX = (int)((x - startOutlineWidth) * ratioWidth) / 2;
                    int itBody = itBodyX / 2 + itYBody;
                    int indexByte = itBodyX % 2;

                    if (itBody < bodyFrame.Length) {
                        byte[] bodyBytes = BitConverter.GetBytes(bodyFrame[itBody]);
                        alpha = (byte)(255 - bodyBytes[indexByte]);
                    }

                    byte[] firstBytes = BitConverter.GetBytes(colorFrame[it]);
                    byte[] secondBytes = BitConverter.GetBytes(colorFrame[it + 1]);

                    currentFrameData.ColorImage[byteCounter++] = secondBytes[0]; // B
                    currentFrameData.ColorImage[byteCounter++] = secondBytes[1]; // R
                    currentFrameData.ColorImage[byteCounter++] = firstBytes[0]; // G
                    currentFrameData.ColorImage[byteCounter++] = alpha; // A
                }

                for (int x = startOutlineWidth - 1; x >= 0; x--) {

                    currentFrameData.ColorImage[byteCounter++] = 0; // B
                    currentFrameData.ColorImage[byteCounter++] = 0; // R
                    currentFrameData.ColorImage[byteCounter++] = 0; // G
                    currentFrameData.ColorImage[byteCounter++] = 0; // A
                }
            }

        }

        /// <summary>
        /// Metoda generuje obraz z kamery RGB.
        /// </summary>
        /// <param name="currentFrameData">Referencja, w której są przechowywane dane z urządzenia.</param>
        /// <param name="frame">Ramka danych z urządzenia.</param>
        private void GetRGBAImage(ref BackgroundData currentFrameData, Frame frame) {

            Capture bodyFrameCapture = frame.Capture;

            Image colorImage = bodyFrameCapture.Color;

            currentFrameData.ColorImageHeight = colorImage.HeightPixels;
            currentFrameData.ColorImageWidth = colorImage.WidthPixels;
            currentFrameData.ColorImageSize = currentFrameData.ColorImageHeight * currentFrameData.ColorImageWidth * 4;

            if (!readFirstFrame) {
                readFirstFrame = true;
                initialTimestamp = colorImage.DeviceTimestamp;
                if (currentFrameData.ColorImage.Length != currentFrameData.ColorImageSize) {
                    currentFrameData.ColorImage = new byte[currentFrameData.ColorImageSize];
                }
            }
            currentFrameData.TimestampInMs = (float)(colorImage.DeviceTimestamp - initialTimestamp).TotalMilliseconds;

            Span<ushort> colorFrame = MemoryMarshal.Cast<byte, ushort>(colorImage.Memory.Span);

            int byteCounter = 0;

            for (int it = (int)(colorFrame.Length) - 1; it >= 0 && byteCounter < currentFrameData.ColorImageSize; it -= 2) {
                byte[] firstBytes = BitConverter.GetBytes(colorFrame[it - 1]);
                byte[] secondBytes = BitConverter.GetBytes(colorFrame[it]);


                currentFrameData.ColorImage[byteCounter++] = firstBytes[0]; // B
                currentFrameData.ColorImage[byteCounter++] = firstBytes[1]; // R
                currentFrameData.ColorImage[byteCounter++] = secondBytes[0]; // G
                currentFrameData.ColorImage[byteCounter++] = 255; // A

            }
        }

        /// <summary>
        /// Metoda, w która jest wywoływana w osobnym wątku, pobiera dane z urządzenia
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <param name="deviceConfiguration"></param>
        private void RunBackgroundThreadAsync(int id, CancellationToken token, DeviceConfiguration deviceConfiguration) {
            try {
                status = KinectStatus.Starting;
                Debug.Log("Starting body tracker background thread.");
                BackgroundData currentFrameData = new BackgroundData();
                GetKinectData getKinectData = GetCurrentMethod();

                using (Device device = Device.Open(id)) {
                    device.StartCameras(deviceConfiguration);
                    Debug.Log("Open K4A device successful. id " + id + "sn:" + device.SerialNum);
                    Calibration deviceCalibration = device.GetCalibration();

                    using (Tracker tracker = Tracker.Create(deviceCalibration, new TrackerConfiguration() { ProcessingMode = TrackerProcessingMode.Cuda, SensorOrientation = SensorOrientation.Default })) {
                        Debug.Log("Body tracker created.");
                        while (!token.IsCancellationRequested) {
                            using (Capture sensorCapture = device.GetCapture()) {
                                tracker.EnqueueCapture(sensorCapture);
                            }
                            using (Frame frame = tracker.PopResult(TimeSpan.Zero, throwOnTimeout: false)) {
                                if (frame == null) {
                                    Debug.Log("Pop result from tracker timeout!");
                                } else {
                                    status = KinectStatus.Running;
                                    currentFrameData.NumOfBodies = frame.NumberOfBodies;
                                    for (uint i = 0; i < currentFrameData.NumOfBodies; i++) {
                                        currentFrameData.Bodies[i].CopyFromBodyTrackingSdk(frame.GetBody(i), deviceCalibration);
                                    }
                                    getKinectData?.Invoke(ref currentFrameData, frame);
                                    SetCurrentFrameData(ref currentFrameData);
                                }
                            }
                        }
                        status = KinectStatus.Off;
                        Debug.Log("Dispose of tracker now!");
                        tracker.Dispose();
                    }
                    device.Dispose();
                }
            } catch (Exception e) {
                Debug.LogError($"catching exception for background thread {e.Message}");
                status = KinectStatus.OffWithError;
            }
        }

        /// <summary>
        /// Delegat służacy do przekazywania metody, która generuje obraz z urządzenia.
        /// </summary>
        /// <param name="currentFrameData">Referencja, w której są przechowywane dane z urządzenia.</param>
        /// <param name="frame">Ramka danych z urządzenia.</param>
        delegate void GetKinectData(ref BackgroundData data, Frame frame);

    }

    /// <summary>
    /// Informacja o statusie urządzenia.
    /// </summary>
    public enum KinectStatus {
        Off, 
        Starting, 
        OffWithError,
        Running 
    }

    /// <summary>
    /// Rodzaj pobierania obrazu z urządzenia.
    /// </summary>
    public enum KinectImageSettings {
        Outline,
        RGB,
        OutlineRGB,
        None
    }
}