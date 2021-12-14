using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualKey : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    static public VirtualKeyboard _Keybord = null;
    public enum kType 
    { 
        kCharacter, 
        kOther, 
        kReturn, 
        kSpace, 
        kBackspace, 
        kShift, 
        kTab, 
        kCapsLock, 
        kHangul,
        kSymbol_Star,
        kSymbol_Heart,
    }
    public float AlphaHitTestMinimumThreshold;
    public char KeyCharacter;
    public kType KeyType = kType.kCharacter;
    private bool mKeepPresed;


    public bool KeepPressed
    {
        set { mKeepPresed = value; }
        get { return mKeepPresed; }
    }

    // private bool mIsPressed = false;
    private Text mKeyText;
    private Text mShiftedText = null;
    private Text mLanguageBtnTxtKor = null;
    private Text mLanguageBtnTxtEng = null;


    static Color Color_Deactivate = new Color(0.4588236f, 0.4588236f, 0.4588236f);

    private void Awake()
    {
        if (AlphaHitTestMinimumThreshold > float.Epsilon)
        {
            Image image = GetComponent<Image>();

            if (image != null)
            {
                image.alphaHitTestMinimumThreshold = AlphaHitTestMinimumThreshold;
            }
        }

        //mShiftedText = transform.Find("ShiftedText")?.GetComponent<Text>();
        Transform t = transform.Find("ShiftedText");
        if(t != null)
        {
            mShiftedText = t.GetComponent<Text>();
        }

        if (KeyType == kType.kHangul)
        {
            //mLanguageBtnTxtKor = transform.Find("Kor").GetComponent<Text>();
            //mLanguageBtnTxtEng = transform.Find("Eng").GetComponent<Text>();
        }

        var txtTransform = transform.Find("Text");

        if (txtTransform != null)
        {
            mKeyText = txtTransform.GetComponent<Text>();
        }


        if (KeyType == kType.kOther)
        {
            //mShiftedText = mSubTransform.Find("ShiftedChar").GetComponent<Text>();
        }
    }
	void Start () {
        UnityEngine.UI.Button _button = gameObject.GetComponent<UnityEngine.UI.Button>();
        if(_button != null)
        {
            /*
            if(mSubTransform != null)
            {
                var targetGraphic = mSubTransform.GetComponent<Image>();
                _button.targetGraphic = targetGraphic;
            }
            */

            _button.onClick.AddListener(onKeyClick);
        }
    }

    void onKeyClick()
    {
        //VirtualKeyboard _keybord = GameObject.FindObjectOfType< VirtualKeyboard>();
        if(_Keybord != null)
        {
            _Keybord.KeyDown(this);
        }
    }

    // Update is called once per frame
    void Update () {

	    if(KeepPressed)
        {
            //do something
        }

        switch (KeyType)
        {
            case kType.kCharacter:
                {
                    if (_Keybord.Language == VirtualKeyboard.kLanguage.kKorean)
                    {
                        if (mShiftedText != null)
                        {
                            if (!mShiftedText.gameObject.activeSelf)
                            {
                                mShiftedText.gameObject.SetActive(true);
                            }
                        }

                        if (_Keybord.PressedShift)
                        {
                            if (mShiftedText != null)
                            {
                                mShiftedText.text = AutomateKR.GetHangulSound(KeyCharacter).ToString();
                                mKeyText.text = AutomateKR.GetHangulSound(char.ToUpper(KeyCharacter)).ToString();
                            }
                            else
                            {
                                mKeyText.text = AutomateKR.GetHangulSound(char.ToUpper(KeyCharacter)).ToString();
                            }
                        }
                        else
                        {
                            if (mShiftedText != null)
                            {
                                mShiftedText.text = AutomateKR.GetHangulSound(char.ToUpper(KeyCharacter)).ToString();
                            }
                            mKeyText.text = AutomateKR.GetHangulSound(KeyCharacter).ToString();
                        }
                    }
                    else
                    {
                        if (_Keybord.CapLockOn || _Keybord.PressedShift)
                        {
                            mKeyText.text = char.ToUpper(KeyCharacter).ToString();
                        }
                        else
                        {
                            mKeyText.text = KeyCharacter.ToString();
                        }

                        if (mShiftedText != null)
                        {
                            if (mShiftedText.gameObject.activeSelf)
                            {
                                mShiftedText.gameObject.SetActive(false);
                            }
                        }
                    }
                }
                break;
            case kType.kOther:
                {
                    if (mShiftedText != null)
                    {
                        if (_Keybord.PressedShift)
                        {
                            mKeyText.color = Color_Deactivate;
                            mShiftedText.color = Color.black;
                        }
                        else
                        {
                            mKeyText.color = Color.black;
                            mShiftedText.color = Color_Deactivate;
                        }
                    }
                }
                break;
            case kType.kShift:
                {
                    if (_Keybord.PressedShift)
                    {
                        if (mKeyText != null)
                        {
                            mKeyText.color = Color.black;
                        }
                    }
                    else
                    {
                        if (mKeyText != null)
                        {
                            mKeyText.color = Color_Deactivate;
                        }
                    }
                }
                break;
            case kType.kCapsLock:
                {
                    if (_Keybord.CapLockOn)
                    {
                        mKeyText.color = Color.white;
                    }
                    else
                    {
                        mKeyText.color = Color_Deactivate;
                    }
                }
                break;
            case kType.kHangul:
                {
                    if (_Keybord.Language == VirtualKeyboard.kLanguage.kKorean)
                    {
                      //  mLanguageBtnTxtKor.color = Color.black;
                      //  mLanguageBtnTxtEng.color = Color_Deactivate;
                    }
                    else
                    {
                      //  mLanguageBtnTxtKor.color = Color_Deactivate;
                      //  mLanguageBtnTxtEng.color = Color.black;
                    }
                }
                break;
        }
    }

    public bool HasShiftedChar()
    {
        return mShiftedText != null;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //mIsPressed = true;
	}

    public void OnPointerUp(PointerEventData eventData)
    {
        //mIsPressed = false;
    }

    [ExecuteInEditMode]
    public void GetKeyCharacterFromObjectName()
    {
        string name = transform.name;

        if(name.Length > 0)
        {
            char[] array = name.ToCharArray(0, 1);

            KeyCharacter = array[0];
        }
    }
}
