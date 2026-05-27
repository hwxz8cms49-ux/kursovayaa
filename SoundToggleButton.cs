using UnityEngine;
using UnityEngine.UI; 

public class SoundToggleButton : MonoBehaviour
{
    public Image _Image;    
    public Sprite SoundOn;  
    public Sprite SoundOff; 

    private Button _Button;

    void Start()
    {
        _Button = GetComponent<Button>();

        _Button.onClick.AddListener(OnButtonClick);

        UpdateIcon();
    }
    private void OnEnable()
    {
        UpdateIcon();
    }
    void OnButtonClick()
    {
        if (Sound.Instance != null)
        {
            Sound.Instance.ToggleMusic();

        }
        UpdateIcon();
    }

    void UpdateIcon()
    {
        if (Sound.Instance != null && _Image != null)
        {
            if (Sound.Instance.IsMuted())
            {
                _Image.sprite = SoundOff;
            }
            else
            {
                _Image.sprite = SoundOn;
            }
        }
    }
}