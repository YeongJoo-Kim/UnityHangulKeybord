using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class VirtualKeyboard : MonoBehaviour {

    public enum kLanguage
    {
        [InspectorName("한글")]
        kKorean,
        [InspectorName("영어")]
        kEnglish
    };

    [System.Serializable]
    public class ReturnEvent : UnityEvent<string> { }
    public delegate bool VirtualKeyboardReturnDelegate(string text);

    public kLanguage DefaultLanguage = kLanguage.kKorean;
    public VirtualKeyboardReturnDelegate OnReturnDelegate { get; set; }

    public ReturnEvent OnReturnEventHandler;
    
    public VirtualTextInputBox TextInputBox = null;
    public int MaxTextCount = 20;


    protected AudioSource mAudioSource;
    protected bool mPressShift = false;
    protected bool mCapsLocked = false;
    protected kLanguage mLanguage = kLanguage.kKorean;
    protected Dictionary<char, char> CHARACTER_TABLE = new Dictionary<char, char>
    {
        {'1', '!'}, {'2', '@'}, {'3', '#'}, {'4', '$'}, {'5', '%'},{'6', '^'}, {'7', '&'}, {'8', '*'}, {'9', '('},{'0', ')'},
        { '`', '~'},   {'-', '_'}, {'=', '+'}, {'[', '{'}, {']', '}'}, {'\\', '|'}, {',', '<'}, {'.', '>'}, {'/', '?'}
    };



    void Awake()
    {
        VirtualKey._Keybord = this;

        mAudioSource = GetComponent<AudioSource>();

    }
	// Use this for initialization
    void Start() {

    }

    private void OnEnable()
    {
        mLanguage = DefaultLanguage;
        mPressShift = false;
	}

	// Update is called once per frame
    void Update() {

	}

    public void Clear()
    {
        if (TextInputBox != null)
        {
            TextInputBox.Clear();
        }
    }

    void OnGUI()
    {
        //Event e = Event.current;
        //if (e.isKey)
        //  Debug.Log("Detected key code: " + e.keyCode);
    }

    public void PlayKeyAudio()
    {
        if (mAudioSource != null)
        {
            mAudioSource.Play();
        }
    }

    public void KeyDown(VirtualKey _key)
    {
        if (_key.KeyType != VirtualKey.kType.kReturn)
        {
            PlayKeyAudio();
        }


        if (TextInputBox != null)
        {
            switch (_key.KeyType)
            {
                case VirtualKey.kType.kShift:
                    {
                        mPressShift = !mPressShift;
                    }
                    break;
                case VirtualKey.kType.kCapsLock:
                    {
                        mCapsLocked = !mCapsLocked;
                    }
                    break;
                case VirtualKey.kType.kHangul:
                    {
                        if (mLanguage == kLanguage.kKorean) mLanguage = kLanguage.kEnglish;
                        else mLanguage = kLanguage.kKorean;
                    }
                    break;
                case VirtualKey.kType.kSpace:
                    {
                        if (TextInputBox.TextField.Length <= MaxTextCount)
                        {
                            TextInputBox.KeyDown(_key);
                        }
                    }
                    break;
                case VirtualKey.kType.kBackspace:
                    {
                        TextInputBox.KeyDown(_key);
                    }
                    break;
                case VirtualKey.kType.kReturn:
                    {
                        if(OnReturnEventHandler != null)
                        {
                            //OnReturnEventHandler(TextInputBox.TextField);
                            
                        }
                        //do somehing
                        //OnReturnEventHandler?.Invoke(TextInputBox.TextField);

                        if((bool)OnReturnDelegate?.Invoke(TextInputBox.TextField))
                        {
                            PlayKeyAudio();
                        }
                    }
                    break;
                case VirtualKey.kType.kCharacter:
                    {
                        if (TextInputBox.TextField.Length <= MaxTextCount)
                        {
                        char keyCharacter = _key.KeyCharacter;
                        if (mPressShift)
                        {
                            keyCharacter = char.ToUpper(keyCharacter);
                            mPressShift = false;
                        }

                        if (mLanguage == kLanguage.kKorean)
                        {
                            TextInputBox.KeyDownHangul(keyCharacter);
                        }
                        else if (mLanguage == kLanguage.kEnglish)
                        {
                                if(mCapsLocked)
                                {
                                    keyCharacter = char.ToUpper(keyCharacter);
                                }
                            TextInputBox.KeyDown(keyCharacter);
                        }
                    }
                        
                    }
                    break;
                case VirtualKey.kType.kOther:
                    {
                        if (TextInputBox.TextField.Length <= MaxTextCount)
                        {
                        char keyCharacter = _key.KeyCharacter;
                        if (mPressShift)
                        {
                                if (_key.HasShiftedChar())
                                {
                            keyCharacter = CHARACTER_TABLE[keyCharacter];
                                }
                            mPressShift = false;
                        }
                        TextInputBox.KeyDown(keyCharacter);
                    }
                    }
                    break;
                case VirtualKey.kType.kSymbol_Star:
                    //https://www.unicodepedia.com/groups/miscellaneous-symbols/
                    /*WHITE STAR*/
                    TextInputBox.KeyDown('\u2606');
                    break;
                case VirtualKey.kType.kSymbol_Heart:
                    /*WHITE HEART*/
                    TextInputBox.KeyDown('\u2661');
                    break;

            }

            }
    }

    public bool PressedShift
    {
        get
        {
            return mPressShift;
        }
    }

    public bool CapLockOn
    {
        get
        {
            return mCapsLocked;
        }
    }

    public kLanguage Language 
    {
        get
        {
            return mLanguage;
        }
    }
}
