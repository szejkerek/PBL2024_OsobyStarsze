using System;
using System.Runtime.Serialization;
using Microsoft.Azure.Kinect.Sensor;

namespace Plugins.KinectModule.Data {

    /// <summary>
    /// Klasa przechowująca dane ramki z urządzenia Azure Kinect.
    /// </summary>
    [Serializable]
    public class BackgroundData {

        /// <summary>
        /// Czas w Ms, określający różnicę czasu pobrania od ostatniego razu.
        /// </summary>
        public float TimestampInMs { get; set; }

        /// <summary>
        /// Przechowuje informacje o obrazie.
        /// </summary>
        public byte[] ColorImage { get; set; }

        /// <summary>
        /// Szerokość w pikselach obrazu zapisanego w ColorImage.
        /// </summary>
        public int ColorImageWidth { get; set; }
        /// <summary>
        /// Wysokość w pikselach obrazu zapisanego w ColorImage.
        /// </summary>
        public int ColorImageHeight { get; set; }
        /// <summary>
        /// Ilość pikseli na dany obraz.
        /// </summary>
        public int ColorImageSize { get; set; }

        /// <summary>
        /// Ilość wykrytych ciał.
        /// </summary>
        public ulong NumOfBodies { get; set; }

        /// <summary>
        /// Przechowuje informacje o każdym wykrytej postaci ludzkiej.
        /// </summary>
        public Body[] Bodies { get; set; }

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="maxBodiesCount">Maksymalna ilość wykrytych postaci ludzkich.</param>
        /// <param name="maxJointsSize">Maksymalna ilość wykrytch stawów.</param>
        /// <param name="maxColorImageSize">Maksymalna ilość pikseli w obrazku. </param>
        public BackgroundData(int maxBodiesCount = 20, int maxJointsSize = 100, long maxColorImageSize = 1280 * 720 * 4) {
            ColorImage = new byte[maxColorImageSize];

            Bodies = new Body[maxBodiesCount];
            for (int i = 0; i < maxBodiesCount; i++) {
                Bodies[i] = new Body(maxJointsSize);
            }
        }
    }

}

