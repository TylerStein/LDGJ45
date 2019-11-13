using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelController : MonoBehaviour
{
    public enum InfoPanelState
    {
        INVISIBLE = 0,
        FADEIN = 1,
        WAIT = 2,
        FADEOUT = 3
    }


    [SerializeField] public Text displayText;
    [SerializeField] public Image displayPanel;

    [SerializeField] public InfoPanelState state;

    [SerializeField] public float displayTime = 5.0f;
    [SerializeField] public float fadeInTime = 0.5f;
    [SerializeField] public float fadeOutTime = 0.5f;

    [SerializeField] private float _alpha = 0f;
    [SerializeField] private float _timer = 0f;

    public void Start() {
        displayText.color = new Color(1f, 1f, 1f, 0f);
        displayPanel.color = new Color(1f, 1f, 1f, 0f);
    }

    public void Display(string text) {
        displayText.text = text;
        state = InfoPanelState.FADEIN;
    }

    public void Update() {
        if (state == InfoPanelState.FADEIN) {
            _timer += Time.deltaTime;
            _alpha = _timer != 0 ? fadeInTime / _timer : 0f;
            if (_timer >= displayTime) {
                _alpha = 1f;
                _timer = 0f;
                state = InfoPanelState.WAIT;
            }
            UpdateDisplay();
        } else if (state == InfoPanelState.WAIT) {
            _timer += Time.deltaTime;
            if (_timer >= displayTime) {
                _timer = 0f;
                state = InfoPanelState.FADEOUT;
            }
        } else if (state == InfoPanelState.FADEOUT) {
            _timer += Time.deltaTime;
            _alpha = 1f - (_timer != 0 ? fadeOutTime / _timer : 0f);
            if (_timer <= 0f) {
                _timer = 0f;
                _alpha = 0f;
                state = InfoPanelState.INVISIBLE;
            }
            UpdateDisplay();
        }
    }

    public void UpdateDisplay() {
        displayText.color = new Color(1f, 1f, 1f, _alpha);
        displayPanel.color = new Color(1f, 1f, 1f, _alpha);
    }
}
