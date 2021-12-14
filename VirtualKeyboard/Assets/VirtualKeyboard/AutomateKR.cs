using UnityEngine;
using System.Collections;
using System.Collections.Generic;



//public class AutomateKR : MonoBehaviour {
public class AutomateKR {

    public static int KEY_CODE_SPACE = -1;		// 띄어쓰기
	public static int KEY_CODE_ENTER = -2;		// 내려쓰기
	public static int KEY_CODE_BACKSPACE = -3;		// 지우기

	public static Dictionary<char, int> HANGULE_KEY_TABLE = new Dictionary<char, int>
	{
		{'q', 7}, 	{'Q', 8},
		{'w', 12}, 	{'W', 13},
		{'e', 3}, 	{'E', 4},
		{'r', 0},	{'R', 1},
		{'t', 9},	{'T', 10},
		{'y', 31},	{'Y', 31},
		{'u', 25},	{'U', 25},
		{'i', 21},	{'I', 21},
		{'o', 20},	{'O', 22},
		{'p', 24},	{'P', 26},

		{'a', 6},	{'A', 6},
		{'s', 2},	{'S', 2},
		{'d', 11},	{'D', 11},
		{'f', 5},	{'F', 5},
		{'g', 18},	{'G', 18},
		{'h', 27},	{'H', 27},
		{'j', 23},	{'J', 23},
		{'k', 19},	{'K', 19},
		{'l', 39},	{'L', 39},

		{'z', 15}, 	{'Z', 15},
		{'x', 16},	{'X', 16},
		{'c', 14},	{'C', 14},
		{'v', 17},	{'V', 17},
		{'b', 36},	{'B', 36},
		{'n', 32},	{'N', 32},
		{'m', 37},	{'M', 37},
	};

    // 초성, 중성, 종성 테이블.
    static string SOUND_TABLE =
    /* 초성 19자 0 ~ 18 */
    "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ" +
    /* 중성 21자 19 ~ 39 */
    "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ" +
    /* 종성 28자 40 ~ 67 */
    " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";

    public enum HAN_STATUS		// 단어조합상태
    {
        HS_FIRST = 0,		// 초성
        HS_FIRST_V,			// 자음 + 자음 
        HS_FIRST_C,			// 모음 + 모음
        HS_MIDDLE_STATE,	// 초성 + 모음 + 모음
        HS_END,				// 초성 + 중성 + 종성
        HS_END_STATE,		// 초성 + 중성 + 자음 + 자음
        HS_END_EXCEPTION	// 초성 + 중성 + 종성(곁자음)
    };

    public string ingWord;		    // 작성중 글자
    public string completeText;	    // 완성 문자열

    const int BASE_CODE = 0xac00;		// 기초음성(가)
    const int LIMIT_MIN = 0xac00;		// 음성범위 MIN(가)
    const int LIMIT_MAX = 0xd7a3;		// 음성범위 MAX

    HAN_STATUS m_nStatus;		        // 단어조합상태
	int[]	m_nPhonemez = new int[5];   // 음소[초,중,종,곁자음1,곁자음2]

	string	m_completeWord;	// 완성글자



    // 초성 합성 테이블
    int[,] MIXED_CHO_CONSON = new int[14,3]
    {
	    { 0, 0,15}, // ㄱ,ㄱ,ㅋ
	    {15, 0, 1}, // ㅋ,ㄱ,ㄲ
	    { 1, 0, 0}, // ㄲ,ㄱ,ㄱ

	    { 3, 3,16}, // ㄷ,ㄷ,ㅌ
	    {16, 3, 4}, // ㅌ,ㄷ,ㄸ
	    { 4, 3, 3}, // ㄸ,ㄷ,ㄷ

	    { 7, 7,17}, // ㅂ,ㅂ,ㅍ
	    {17, 7, 8}, // ㅍ,ㅂ,ㅃ
	    { 8, 7, 7}, // ㅃ,ㅂ,ㅂ

	    { 9, 9,10}, // ㅅ,ㅅ,ㅆ
	    {10, 9, 9}, // ㅆ,ㅅ,ㅅ

	    {12,12,14}, // ㅈ,ㅈ,ㅊ
	    {14,12,13}, // ㅊ,ㅈ,ㅉ
	    {13,12,12}  // ㅉ,ㅈ,ㅈ
    };

    // 초성,중성 모음 합성 테이블
    int[,] MIXED_VOWEL = new int[22,3] {
	    {19,19,21},	// ㅏ,ㅏ,ㅑ
	    {21,19,19},	// ㅑ,ㅏ,ㅏ

	    {19,39,20},	// ㅏ,ㅣ,ㅐ
	    {21,39,22},	// ㅑ,ㅣ,ㅒ

	    {23,23,25},	// ㅓ,ㅓ,ㅕ
	    {25,23,23},	// ㅕ,ㅓ,ㅓ

	    {23,39,24},	// ㅓ,ㅣ,ㅔ
	    {25,39,26},	// ㅕ,ㅣ,ㅖ

	    {27,27,31},	// ㅗ,ㅗ,ㅛ
	    {31,27,27},	// ㅛ,ㅗ,ㅗ

	    {27,19,28},	// ㅗ,ㅏ,ㅘ
        {27,20,29},	// ㅗ,ㅐ,ㅙ
	    {28,39,29},	// ㅘ,ㅣ,ㅙ

	    {27,39,30},	// ㅗ,ㅣ,ㅚ

	    {32,32,36},	// ㅜ,ㅜ,ㅠ
	    {36,32,32},	// ㅠ,ㅜ,ㅜ

	    {32,23,33},	// ㅜ,ㅓ,ㅝ
	    {33,39,34},	// ㅝ,ㅣ,ㅞ

	    {32,39,35},	// ㅜ,ㅣ,ㅟ

	    {39,39,37},	// ㅣ,ㅣ,ㅡ
	    {37,39,38},	// ㅡ,ㅣ,ㅢ
	    {38,39,39}	// ㅢ,ㅣ,ㅣ
    };

    // 종성 합성 테이블
    int[,] MIXED_JONG_CONSON = new int[11, 3] {

	    //{41,41,64}, // ㄱ,ㄱ,ㅋ
	    //{64,41,42}, // ㅋ,ㄱ,ㄲ
	    //{42,41,41}, // ㄲ,ㄱ,ㄱ
 
	    {41,59,43}, // ㄱ,ㅅ,ㄳ
 
	    {44,62,45}, // ㄴ,ㅈ,ㄵ
	    {44,67,46}, // ㄴ,ㅎ,ㄶ
 
	    //{47,47,65}, // ㄷ,ㄷ,ㅌ
	    //{65,47,47}, // ㅌ,ㄷ,ㄷ
 
	    {48,41,49}, // ㄹ,ㄱ,ㄺ
	    {48,56,50}, // ㄹ,ㅁ,ㄻ
 
	    {48,57,51}, // ㄹ,ㅂ,ㄼ
	    {51,57,54}, // ㄼ,ㅂ,ㄿ
 
	    {48,59,52}, // ㄹ,ㅅ,ㄽ
	    {48,65,53}, // ㄹ,ㄷ,ㄾ	
	    {48,67,55}, // ㄹ,ㅎ,ㅀ
 
	    //{57,57,66}, // ㅂ,ㅂ,ㅍ
	    //{66,57,57}, // ㅍ,ㅂ,ㅂ
 
	    {57,59,58} // ㅂ,ㅅ,ㅄ
 
	    //{59,59,60}, // ㅅ,ㅅ,ㅆ
	    //{60,59,59}, // ㅆ,ㅅ,ㅅ
 
	    //{62,62,63}, // ㅈ,ㅈ,ㅊ
	    //{63,62,62}  // ㅊ,ㅈ,ㅈ
    };

    // 종성 분해 테이블
    int[,] DIVIDE_JONG_CONSON = new int[13,3] {
	    {41,41,42}, // ㄱ,ㄱ,ㄲ
	    {41,59,43}, // ㄱ,ㅅ,ㄳ
	    {44,62,45}, // ㄴ,ㅈ,ㄵ
	    {44,67,46}, // ㄴ,ㅎ,ㄶ
	    {48,41,49}, // ㄹ,ㄱ,ㄺ
	    {48,56,50}, // ㄹ,ㅁ,ㄻ
	    {48,57,51}, // ㄹ,ㅂ,ㄼ
	    {48,66,54}, // ㄹ,ㅍ,ㄿ
	    {48,59,52}, // ㄹ,ㅅ,ㄽ
	    {48,65,53}, // ㄹ,ㅌ,ㄾ	
	    {48,67,55}, // ㄹ,ㅎ,ㅀ
	    {57,59,58}, // ㅂ,ㅅ,ㅄ
	    {59,59,60}  // ㅅ,ㅅ,ㅆ
        
    };

    int currentCode;// 포인터 대체

   // 버퍼 초기화
    public void Clear()
    {
	    m_nStatus		= HAN_STATUS.HS_FIRST;
	    completeText	= "";
	    ingWord			= null;
	    m_completeWord	= null;
    }

    static public char GetHangulSound(char c)
    {
        int index = -1;
        if(HANGULE_KEY_TABLE.ContainsKey(c))
        {
            index = HANGULE_KEY_TABLE[c];
        }

        if(index < 0)
        {
            return '\0';
        }

        return SOUND_TABLE[index];
	}


    public HAN_STATUS GetStatus()
    {
        return m_nStatus;
	}

    public void SetKeyString(char str)
    {
        m_nStatus = HAN_STATUS.HS_FIRST;
        if (ingWord != null)
        {
            completeText += ingWord;
            completeText += str;
        }
        else
            completeText += str;

        ingWord = null;
        
    }

    public string SetKeyCode(char _key)
    {
        return SetKeyCode(HANGULE_KEY_TABLE[_key]);
    }

    
    public string SetKeyCode(int nKeyCode)
    {
        m_completeWord = null;

        // 특수키 입력
        if (nKeyCode < 0)
        {
            m_nStatus = HAN_STATUS.HS_FIRST;

            if (nKeyCode == KEY_CODE_SPACE) // 띄어쓰기
            {
                if (ingWord != null){
                    completeText += ingWord;
                    completeText += " ";
                }
                else
                    completeText += " ";

                ingWord = null;
            }
            else if (nKeyCode == KEY_CODE_ENTER) // 내려쓰기
            {
                if (ingWord != null)
                    completeText += ingWord;

                completeText += "\r\n";

                ingWord = null;
            }
            else if (nKeyCode == KEY_CODE_BACKSPACE) // 지우기
            {
                if (ingWord == null) // 쓰던 문자가 없을때
                {
                    if (completeText != null)
                    {
                        if (completeText.Length > 0)
                        {
                            int n = completeText.LastIndexOf("\n");
                            if (n == -1)
                                completeText = completeText.Remove(completeText.Length - 1);
                            else
                                completeText = completeText.Remove(completeText.Length - 2);
                        }
                    }
                }
                else
                {
                    m_nStatus = DownGradeIngWordStatus(ingWord);
                }
            }

            return m_completeWord;
        }


        // 특수키가 아닌 경우
        switch (m_nStatus)
        {
            case HAN_STATUS.HS_FIRST:// 초성
                // 초성
                m_nPhonemez[0] = nKeyCode;
                ingWord = new string(SOUND_TABLE[m_nPhonemez[0]], 1);
                m_nStatus = (nKeyCode > 18) ? HAN_STATUS.HS_FIRST_C : HAN_STATUS.HS_FIRST_V;
                break;

        

            case HAN_STATUS.HS_FIRST_C:// 모음 + 모음
                // 모음 + 모음
                m_completeWord = new string(SOUND_TABLE[m_nPhonemez[0]], 1); 
                m_nPhonemez[0] = nKeyCode;
                if (nKeyCode > 18)	// 모음
                {
                    
                }
                else				// 자음
                {
                    
                    m_nStatus = HAN_STATUS.HS_FIRST_V;
                }
                break;

            case HAN_STATUS.HS_FIRST_V:// 자음 + 자음 
                // 자음 + 자음
                if (nKeyCode > 18)	// 모음
                {
                    m_nPhonemez[1] = nKeyCode;
                    m_nStatus = HAN_STATUS.HS_MIDDLE_STATE;
                }
                else // 자음
                {
                    //	if(!MixInitial(nKeyCode))
                    {
                        m_completeWord = new string(SOUND_TABLE[m_nPhonemez[0]], 1); 
                        m_nPhonemez[0] = nKeyCode;
                        m_nStatus = HAN_STATUS.HS_FIRST_V;
                    }
                }
                break;

            case HAN_STATUS.HS_MIDDLE_STATE:// 초성 + 모음 + 모음
                // 초성 + 모음 + 모음
                if (nKeyCode > 18)
                {
                    if (MixVowel(m_nPhonemez[1], nKeyCode) == false) // 모음+모음 합성 안될때 
                    {
                        m_completeWord = CombineHangle(1);
                        m_nPhonemez[0] = nKeyCode;
                        m_nStatus = HAN_STATUS.HS_FIRST_C;
                    }
                    else
                    {
                        m_nPhonemez[1] = currentCode; // 중성 모음+모음 합성
                        m_nStatus = HAN_STATUS.HS_MIDDLE_STATE;
                    }
                }
                else // 자음이 들어올때 
                {
                    int jungCode = ToFinal(nKeyCode);

                    if (jungCode > 0)
                    {
                        m_nPhonemez[2] = jungCode;
                        m_nStatus = HAN_STATUS.HS_END_STATE;
                    }
                    else
                    {
                        m_completeWord = CombineHangle(1);
                        m_nPhonemez[0] = nKeyCode;
                        m_nStatus = (nKeyCode > 18) ? HAN_STATUS.HS_FIRST_C : HAN_STATUS.HS_FIRST_V;
                        //m_nStatus = HAN_STATUS.HS_FIRST;
                    }
                }
                break;

            case HAN_STATUS.HS_END:// 초성 + 중성 + 종성
                
                if (nKeyCode > 18)
                {
                    m_completeWord = CombineHangle(1);
                    m_nPhonemez[0] = nKeyCode;
                    m_nStatus = HAN_STATUS.HS_FIRST;
                }
                else
                {
                    int jungCode = ToFinal(nKeyCode);
                    if (jungCode > 0)
                    {
                        m_nPhonemez[2] = jungCode;
                        m_nStatus = HAN_STATUS.HS_END_STATE;
                    }
                    else
                    {
                        m_completeWord = CombineHangle(1);
                        m_nPhonemez[0] = nKeyCode;
                        m_nStatus = HAN_STATUS.HS_FIRST;
                    }
                }
                break;

            case HAN_STATUS.HS_END_STATE:
                // 초성 + 중성 + 자음(종) + 자음(종)
                if (nKeyCode > 18)
                {
                    m_completeWord = CombineHangle(1);

                    m_nPhonemez[0] = ToInitial(m_nPhonemez[2]);
                    m_nPhonemez[1] = nKeyCode;
                    m_nStatus = HAN_STATUS.HS_MIDDLE_STATE;
                }
                else
                {
                    int jungCode = ToFinal(nKeyCode);
                    if (jungCode > 0)
                    {
                        m_nStatus = HAN_STATUS.HS_END_EXCEPTION;

                        if (!MixFinal(jungCode)) // 종성 자음 합성 무시
                        {
                            m_completeWord = CombineHangle(2);
                            m_nPhonemez[0] = nKeyCode;
                            m_nStatus = HAN_STATUS.HS_FIRST_V;
                        }
                    }
                    else
                    {
                        m_completeWord = CombineHangle(2);
                        m_nPhonemez[0] = nKeyCode;
                        m_nStatus = HAN_STATUS.HS_FIRST_V;
                    }
                }
                break;

            case HAN_STATUS.HS_END_EXCEPTION:
                // 초성 + 중성 + 종성(곁자음)
                if (nKeyCode > 18)
                {
                    DecomposeConsonant();
                    m_nPhonemez[1] = nKeyCode;
                    m_nStatus = HAN_STATUS.HS_MIDDLE_STATE;
                }
                else
                {
                    int jungCode = ToFinal(nKeyCode);
                    if (jungCode > 0)
                    {
                        m_nStatus = HAN_STATUS.HS_END_EXCEPTION;

                        if(!MixFinal(jungCode))
                        {
                            m_completeWord = CombineHangle(2);
                            m_nPhonemez[0] = nKeyCode;
                            m_nStatus = HAN_STATUS.HS_FIRST_V;
                        }
                    }
                    else
                    {
                        m_completeWord = CombineHangle(2);
                        m_nPhonemez[0] = nKeyCode;
                        m_nStatus = HAN_STATUS.HS_FIRST_V;
                    }
                }
                break;
        }

        // 현재 보이는 글자상태
        CombineIngWord(m_nStatus);

        // 완성 문자열 만들기
        if (m_completeWord != null)
            completeText += m_completeWord;

        return m_completeWord;
    }

    
    // 초성으로 변환
    int ToInitial(int nKeyCode)
    {
	    switch(nKeyCode)
	    {
	        case 41: return 0;	// ㄱ
	        case 42: return 1;	// ㄲ
	        case 44: return 2;	// ㄴ
	        case 47: return 3;	// ㄷ
	        case 48: return 5;	// ㄹ
	        case 56: return 6;	// ㅁ
	        case 57: return 7;	// ㅂ
	        case 59: return 9;	// ㅅ
	        case 60: return 10;	// ㅆ
	        case 61: return 11;	// ㅇ
	        case 62: return 12;	// ㅈ
	        case 63: return 14;	// ㅊ
	        case 64: return 15;	// ㅋ
	        case 65: return 16;	// ㅌ
	        case 66: return 17;	// ㅍ
	        case 67: return 18;	// ㅎ
	    }
	    return -1;
    }

    // 종성으로 변환
    int ToFinal(int nKeyCode)
    {
	    switch(nKeyCode)
	    {
	        case 0: return 41;	// ㄱ
	        case 1: return 42;	// ㄲ
	        case 2: return 44;	// ㄴ
	        case 3: return 47;	// ㄷ
	        case 5: return 48;	// ㄹ
	        case 6: return 56;	// ㅁ
	        case 7: return 57;	// ㅂ
	        case 9: return 59;	// ㅅ
	        case 10: return 60;	// ㅆ
	        case 11: return 61;	// ㅇ
	        case 12: return 62;	// ㅈ
	        case 14: return 63;	// ㅊ
	        case 15: return 64;	// ㅋ
	        case 16: return 65;	// ㅌ
	        case 17: return 66;	// ㅍ
	        case 18: return 67;	// ㅎ
	    }
	    return -1;
    }

    // 자음분해
    void DecomposeConsonant()
    {
	    int i = 0;
	    if(m_nPhonemez[3] > 40 || m_nPhonemez[4] > 40)
	    {
		    do
		    {
			    if(DIVIDE_JONG_CONSON[i,2] == m_nPhonemez[2])
			    {
				    m_nPhonemez[3] = DIVIDE_JONG_CONSON[i,0];
				    m_nPhonemez[4] = DIVIDE_JONG_CONSON[i,1];

				    m_completeWord = CombineHangle(3);
				    m_nPhonemez[0]	 = ToInitial(m_nPhonemez[4]);
				    return;
			    }
		    }
		    while(++i< 13);
	    }

	    m_completeWord = CombineHangle(1);
	    m_nPhonemez[0]	 = ToInitial(m_nPhonemez[2]);
    }

    // 초성합성
    bool MixInitial(int nKeyCode)
    {
	    if(nKeyCode >= 19)
		    return false;

	    int i = 0;
	    do
	    {
		    if(MIXED_CHO_CONSON[i,0] == m_nPhonemez[0] && MIXED_CHO_CONSON[i,1] == nKeyCode)
		    {
			    m_nPhonemez[0] = MIXED_CHO_CONSON[i,2];
			    return true;
		    }
	    }
	    while(++i < 14);

	    return false;
    }

    // 종성합성
    bool MixFinal(int nKeyCode)
    {
	    if(nKeyCode <= 40) return false;

	    int i = 0;
	    do
	    {
		    if(MIXED_JONG_CONSON[i,0] == m_nPhonemez[2] && MIXED_JONG_CONSON[i,1] == nKeyCode)
		    {
			    m_nPhonemez[3] = m_nPhonemez[2];
			    m_nPhonemez[4] = nKeyCode;
			    m_nPhonemez[2] = MIXED_JONG_CONSON[i,2];

			    return true;
		    }
	    }
	    while(++i < 11);

	    return false;
    }

    // 모음합성
    bool MixVowel(int currentC, int inputCode)
    {
        currentCode = currentC;
	    int i = 0;
	    do
	    {
		    if(MIXED_VOWEL[i,0] == currentCode && MIXED_VOWEL[i,1] == inputCode)
		    {
//                print(MIXED_VOWEL[i, 2]);
			    currentCode = MIXED_VOWEL[i,2];
			    return true;
		    }
	    }
	    while(++i< 22);

	    return false;
    }

    
    // 한글조합 Check
    string CombineHangle(int cho, int jung, int jong)
    {
	    // 초성 * 21 * 28 + (중성 - 19) * 28 + (종성 - 40) + BASE_CODE;
        return new string(System.Convert.ToChar(BASE_CODE - 572 + jong + cho * 588 + jung * 28), 1);
    }

    string CombineHangle(int status)
    {
	    switch(status)
	    {
	        //초성 + 중성
	        case 1: 
                return CombineHangle(m_nPhonemez[0], m_nPhonemez[1], 40);

	        //초성 + 중성 + 종성
	        case 2: 
                return CombineHangle(m_nPhonemez[0], m_nPhonemez[1], m_nPhonemez[2]);
	
	        //초성 + 중성 + 곁자음01
	        case 3: 
                return CombineHangle(m_nPhonemez[0], m_nPhonemez[1], m_nPhonemez[3]);
	    }
	    return "";
    }

    void CombineIngWord(HAN_STATUS status)
    {
	    switch(m_nStatus)
	    {
	    case HAN_STATUS.HS_FIRST:
        case HAN_STATUS.HS_FIRST_V:
	    case HAN_STATUS.HS_FIRST_C:
            ingWord = new string(SOUND_TABLE[m_nPhonemez[0]], 1);
		    break;

	    case HAN_STATUS.HS_MIDDLE_STATE:
        case HAN_STATUS.HS_END:
		    ingWord = CombineHangle(1);
		    break;

        case HAN_STATUS.HS_END_STATE:
        case HAN_STATUS.HS_END_EXCEPTION:
		    ingWord = CombineHangle(2);
		    break;
	    }
    }

    // 작성중인 한글이 있을경우 삭제
    HAN_STATUS DownGradeIngWordStatus(string word)
    {
//        print("target : " + word + " : "+ word.Length);
        //print("Convert : " + );


        int iWord = System.Convert.ToInt32(char.Parse(word));

        //초성만 있는 경우
        if (iWord < LIMIT_MIN || iWord > LIMIT_MAX)
        {
            ingWord = null;
            return HAN_STATUS.HS_FIRST;
        }

        // 문자코드 체계
        // iWord = firstWord * (21*28)
        //		+ middleWord * 28
        //		+ lastWord * 27
        //		+ BASE_CODE;
        //

        int totalWord = iWord - BASE_CODE;
        int iFirstWord = totalWord / 28 / 21;	//초성
        int iMiddleWord = totalWord / 28 % 21;	//중성
        int iLastWord = totalWord % 28;		//종성

        m_nPhonemez[0] = iFirstWord; //초성저장

        if (iLastWord == 0)	//종성이 없는 경우
        {
            ingWord = new string(SOUND_TABLE[m_nPhonemez[0]], 1);
            return HAN_STATUS.HS_FIRST_V;
        }

        m_nPhonemez[1] = iMiddleWord + 19; //중성저장

        iLastWord += 40;

        for (int i = 0; i < 13; i++)
        {
            
            if (iLastWord == DIVIDE_JONG_CONSON[i,2])
            {
                ingWord = CombineHangle(3);
                m_nPhonemez[2] = DIVIDE_JONG_CONSON[i,0]; // 종성저장
                return HAN_STATUS.HS_END_EXCEPTION;
            }
        }
        ingWord = CombineHangle(1);

        return HAN_STATUS.HS_MIDDLE_STATE;
    }
}