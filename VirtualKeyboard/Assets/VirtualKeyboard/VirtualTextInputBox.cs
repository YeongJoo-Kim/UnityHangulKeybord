using UnityEngine;
using System.Collections;

public class VirtualTextInputBox : MonoBehaviour {

	AutomateKR		mAutomateKR = new AutomateKR();
    protected UnityEngine.UI.InputField mTextField = null;
    public string TextField
    {
        set
        {
            if (mTextField != null)
            {
                mTextField.text = value;
            }
        }
        get
        {
            if (mTextField != null)
            {
                return mTextField.text;
            }
            return "";
        }
    }

    void Start () {
        mTextField = GetComponent<UnityEngine.UI.InputField>();
	}
	
	void Update () {

	}

    public void Clear()
    {
        mAutomateKR.Clear();

        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
    }


    public void KeyDownHangul(char _key)
    {
        mAutomateKR.SetKeyCode(_key);

        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
    }

    public void KeyDown(char _key)
    {
        mAutomateKR.SetKeyString(_key);

        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
    }

    public void KeyDown(VirtualKey _key)
    {
        switch(_key.KeyType)
        {
            case VirtualKey.kType.kBackspace:
                {
                    mAutomateKR.SetKeyCode(AutomateKR.KEY_CODE_BACKSPACE);

                }
                break;
            case VirtualKey.kType.kSpace:
                {
                    mAutomateKR.SetKeyCode(AutomateKR.KEY_CODE_SPACE);
                }
                break;
        }

        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
    }

    public AutomateKR.HAN_STATUS GetStatus()
    {
        return mAutomateKR.GetStatus();
    }
}
