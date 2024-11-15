using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Klasa bazowa do wyświetlania tekstu na ekranie.
/// </summary>
[RequireComponent(typeof(Text))]
public abstract class TextSetter : MonoBehaviour{

    /// <summary>
    /// Zmienna przechowująca referencję do obiektu klasy Text.
    /// </summary>
    Text text;

    /// <summary>
    /// Na stacie działania obiektu wyszukuje komponent Text w obiekcie.
    /// </summary>
    void Awake() {
        text = GetComponent<Text>();    
    }

    /// <summary>
    /// Wyświetla tekst na ekranie.
    /// </summary>
    /// <param name="newText">Treść tekstu.</param>
    public void SetText(string newText) {
        text.text = newText;
    }

    /// <summary>
    /// Zmienia kolor tekstu.
    /// </summary>
    /// <param name="newColor">Nowy kolor tekstu.</param>
    public void SetColor(Color newColor) {
        text.color = newColor;
    }

}
