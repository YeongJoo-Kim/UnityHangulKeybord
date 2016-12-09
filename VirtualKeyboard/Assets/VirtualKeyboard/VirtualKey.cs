using UnityEngine;
using System.Collections;

public class VirtualKey : MonoBehaviour {

    static public VirtualKeyboard _Keybord = null;
    public enum kType { kCharacter, kOther, kReturn, kSpace, kBackspace, kShift, kTab, kCapsLock, kHangul}
    public char KeyCharacter;
    public kType KeyType = kType.kCharacter;
    
    private bool mKeepPresed;
    public bool KeepPressed
    {
        set { mKeepPresed = value; }
        get { return mKeepPresed; }
    }

	// Use this for initialization
	void Start () {
        UnityEngine.UI.Button _button = gameObject.GetComponent<UnityEngine.UI.Button>();
        if(_button != null)
        {
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
	}



    
}
