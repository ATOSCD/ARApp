using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HangulKeyborad : MonoBehaviour
{
    //PanelLoginFocusType Plft;

    public InputField korField;
    ////VirtualKeyboard
    public GameObject wndKeyBoard;

    //public GameObject[] NumberKey;
    public GameObject[] LangKey;
    public GameObject[] LangKey2; //숫자패드쪽

    public GameObject goShiftKey;
    public GameObject goSymbolkey;

    private bool isEngOn = true;
    private bool isSymbolAct = false;
    private bool isShift = false;
    private bool isMan = true; //Gender Info //남녀확인

    private int selectTapNum = 0;

    private int caretPosition = 0;//현재커서포지션

    string[] EngKey = { "q", "w", "e", "r", "t", "y",
        "u","i", "o","p", "a","s", "d","f", "g", "h", "j", "k", "l", "z", "x","c", "v","b", "n","m"};

    string[] EngShiftKey = { "Q", "W", "E", "R", "T", "Y",
        "U", "I", "O","P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "Z", "X","C", "V", "B", "N","M"};

    string[] korKey = { "ㅂ", "ㅈ", "ㄷ", "ㄱ", "ㅅ", "ㅛ",
        "ㅕ","ㅑ", "ㅐ","ㅔ", "ㅁ","ㄴ", "ㅇ","ㄹ", "ㅎ", "ㅗ", "ㅓ", "ㅏ", "ㅣ", "ㅋ", "ㅌ","ㅊ", "ㅍ","ㅠ", "ㅜ","ㅡ"};

    string[] korShiftKey = { "ㅃ", "ㅉ", "ㄸ", "ㄲ", "ㅆ", "ㅛ",
        "ㅕ","ㅑ", "ㅒ","ㅖ", "ㅁ","ㄴ", "ㅇ","ㄹ", "ㅎ", "ㅗ", "ㅓ", "ㅏ", "ㅣ", "ㅋ", "ㅌ","ㅊ", "ㅍ","ㅠ", "ㅜ","ㅡ"};

    string[] NumKey = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };

    string[] SymbolKey = { "\'", "~", "-", "_", "=", "+",
        "\\", "|", "[","]", ";", ":", "'", "\"", "{", "}", "<", ">", "?", ",", ".","/","♥", "★", "●", "■"};

    string[] SymbolKey2 = { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")" };

    //한글키보드
    char[] chosung_index = { 'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' }; //19개
    char[] joongsung_index = { 'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ', 'ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ', 'ㅣ' }; //22개
    char[] jongsung_index = {' ' , 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ', 'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅁ', //28개
          'ㅂ', 'ㅄ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };

    char[] Jcombo_index = { 'ㄳ', 'ㄵ', 'ㄶ', 'ㄺ', 'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅄ' }; //11개 
    string[] Jcombo = { "ㄱㅅ", "ㄴㅈ", "ㄴㅎ", "ㄹㄱ", "ㄹㅁ", "ㄹㅂ", "ㄹㅅ", "ㄹㅌ", "ㄹㅍ", "ㄹㅎ", "ㅂㅅ" };
    char[] Mcombo_index = { 'ㅘ', 'ㅙ', 'ㅚ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅢ' }; //7개 
    string[] Mcombo = { "ㅗㅏ", "ㅗㅐ", "ㅗㅣ", "ㅜㅓ", "ㅜㅔ", "ㅜㅣ", "ㅡㅣ" };

    char chKorInput = ' '; //받은 글자
                           //kor UniCode = (start * 21 + mid )*28 + End + 0xAC00

    //InputKey
    public void OnClicked(Text text)
    {
        OnClickedOnKor(korField, text);
    }

    void OnClickedOnKor(InputField inputfiled, Text text)
    {
        //앞에 글자가 없거나 앞글자가 한글이 아닌경우 -> 그냥 바로 출력
        if (inputfiled.text.Length == 0 || !isKorean(inputfiled.text[inputfiled.text.Length - 1]) || !isKorean(text.text[0]))
        {
            string newText = text.text;
            inputfiled.text = inputfiled.text.Insert(inputfiled.text.Length, newText);
        }
        else//앞에 글자가 있는경우
        {
            chKorInput = text.text[0];
            //자음인지 모음인지 구분
            char JM = isJaOrMo(chKorInput);
            char lasttext = inputfiled.text[inputfiled.text.Length - 1];

            //1. 마지막 글자의 초성,중성,종성과 인덱스 구하기
            char chosung = ' ';
            char joongsung = ' ';
            char jongsung = ' ';

            int jong_idx = -1;
            int joong_idx = -1;
            int cho_idx = -1;

            //마지막 문자가 하나의 자음만 있는 경우 
            if (isJaOrMo(lasttext) == 'J')
            {
                chosung = lasttext;
                joongsung = ' ';
                jongsung = ' ';

                //마지막 문자가 하나의 모음만 있는 경우 
            }
            else if (isJaOrMo(lasttext) == 'M')
            {
                chosung = ' ';
                joongsung = lasttext;
                jongsung = ' ';

            }
            else
            {//마지막 문자가 하나의 자음이나 모음이 아닌경우 

                //마지막 문자에서 AC00을 뺀다
                var lastchar_uni_cal = lasttext - 44032;

                //마지막 문자의 초성, 중성, 종성의 인덱스 구하기 
                //한글음절위치 = ((초성index * 21) + 중성index) * 28 + 종성index
                jong_idx = lastchar_uni_cal % 28;
                joong_idx = Mathf.FloorToInt(lastchar_uni_cal / 28) % 21;
                cho_idx = Mathf.FloorToInt((Mathf.FloorToInt(lastchar_uni_cal / 28)) / 21);

                //마지막 문자의 초성, 중성, 종성 구하기 
                chosung = chosung_index[cho_idx];
                joongsung = joongsung_index[joong_idx];
                jongsung = jongsung_index[jong_idx];
            }

            string str_uni = string.Empty;
            int key_idx = 0;

            //글자 재조합
            if (lasttext == chosung && JM == 'J')//1 . 앞에 자음 + 자음
            {
                string newja = string.Empty;
                newja = newja + chosung;
                newja = newja + chKorInput;

                key_idx = getIndexinArray<string>(Jcombo, newja); // -1 이면 콤보가 없는경우

                if (key_idx != -1)
                {
                    string newText = string.Empty;
                    newText += Jcombo_index[key_idx];
                    inputfiled.text = inputfiled.text.Remove(inputfiled.text.Length - 1);
                    inputfiled.text = inputfiled.text.Insert(inputfiled.text.Length, newText);
                }
                else//콤보아닌경우 그냥 추가
                {
                    string newText = text.text;
                    inputfiled.text = inputfiled.text.Insert(inputfiled.text.Length, newText);
                }

            }
            else if (lasttext == chosung && JM == 'M') // 2. 앞에 자음만 있는 경우 + 모음
            {

                for (int i = 0; i < joongsung_index.Length; ++i)
                {
                    if (joongsung_index[i] == chKorInput)
                    {
                        key_idx = i;
                        break;
                    }
                }

                int indexCombo = -1;
                //앞에 자음이 콤보인 경우 ex) ㄳ
                indexCombo = getIndexinArray<char>(Jcombo_index, lasttext);

                if (indexCombo != -1)
                {
                    //콤보에서도 ㄲ ㄸ ㅃ ㅆ ㅉ  ex) ㄲ + ㅗ = 꼬
                    if (lasttext == 'ㅃ' || lasttext == 'ㅉ' || lasttext == 'ㄸ' || lasttext == 'ㄲ' || lasttext == 'ㅆ')
                    {
                        int Cho_idx = -1;
                        for (int i = 0; i < chosung_index.Length; i++)
                        {
                            if (lasttext == chosung_index[i])
                            {
                                Cho_idx = i;
                                break;
                            }
                        }

                        char newJM = (char)(((Cho_idx * 21) + key_idx) * 28 + 44032);
                        str_uni += newJM;

                        inputfiled.text = inputfiled.text.Remove(inputfiled.text.Length - 1);
                        inputfiled.text = inputfiled.text.Insert(inputfiled.text.Length, str_uni);

                    }
                    else //ex) ㄳ + ㅏ = ㄱ사
                    {
                        char ja1 = Jcombo[indexCombo][0];
                        char ja2 = Jcombo[indexCombo][1];

                        int Cho_idx = -1;
                        for (int i = 0; i < chosung_index.Length; i++)
                        {
                            if (ja2 == chosung_index[i])
                            {
                                Cho_idx = i;
                                break;
                            }
                        }

                        char newJM = (char)(((Cho_idx * 21) + key_idx) * 28 + 44032);

                        str_uni += ja1;
                        str_uni += newJM;

                        inputfiled.text = inputfiled.text.Remove(inputfiled.text.Length - 1);
                        inputfiled.text = inputfiled.text.Insert(inputfiled.text.Length, str_uni);
                    }
                }
                else //  no combo  ex_ ㄱ + ㅏ = 가
                {
                    int Cho_idx = -1;
                    for (int i = 0; i < chosung_index.Length; i++)
                    {
                        if (lasttext == chosung_index[i])
                        {
                            Cho_idx = i;
                            break;
                        }
                    }

                    char newJM = (char)(((Cho_idx * 21) + key_idx) * 28 + 44032);
                    str_uni += newJM;

                    inputfiled.text = inputfiled.text.Remove(inputfiled.text.Length - 1);
                    inputfiled.text = inputfiled.text.Insert(inputfiled.text.Length, str_uni);
                }

            }
            else if (lasttext == joongsung && JM == 'J') //3, 앞에 모음만 있는경우 + 자음
            {

                str_uni += chKorInput;
                inputfiled.text = inputfiled.text.Insert(inputfiled.text.Length, str_uni);
            }
            else if (lasttext == joongsung && JM == 'M')// 4, 앞에 모음만 있는경우 + 모음
            {
                string newMo = string.Empty;
                newMo += lasttext;
                newMo += chKorInput;

                key_idx = getIndexinArray<string>(Mcombo, newMo);

                //combo
                if (key_idx != -1)
                {
                    str_uni += Mcombo_index[key_idx];
                    inputfiled.text = inputfiled.text.Remove(inputfiled.text.Length - 1);
                    inputfiled.text = inputfiled.text.Insert(inputfiled.text.Length, str_uni);
                }
                else // 그냥 뒤에 추가
                {
                    str_uni += chKorInput;
                    inputfiled.text = inputfiled.text.Insert(inputfiled.text.Length, str_uni);
                }

            }
            else if (jongsung != ' ' && JM == 'J')//5.이전 글자가 종성이 있는 경우 + 자음
            {
                string newJong = string.Empty;
                newJong += jongsung;
                newJong += chKorInput;

                key_idx = getIndexinArray<string>(Jcombo, newJong);

                //Combo
                if (key_idx != -1)
                {
                    cho_idx = getIndexinArray<char>(chosung_index, chosung);
                    joong_idx = getIndexinArray<char>(joongsung_index, joongsung);
                    char newJongsung = Jcombo_index[key_idx];
                    jong_idx = getIndexinArray<char>(jongsung_index, newJongsung);


                    char newCh = (char)(((cho_idx * 21) + joong_idx) * 28 + jong_idx + 44032);
                    str_uni += newCh;
                    inputfiled.text = inputfiled.text.Remove(inputfiled.text.Length - 1);
                    inputfiled.text = inputfiled.text.Insert(inputfiled.text.Length, str_uni);
                }
                else
                {
                    str_uni += chKorInput;
                    inputfiled.text = inputfiled.text.Insert(inputfiled.text.Length, str_uni);
                }

            }
            else if (jongsung != ' ' && JM == 'M')//6.이전 글자가 종성이 있는 경우 + 모음
            {
                key_idx = getIndexinArray<char>(Jcombo_index, jongsung);

                //종성이 Combo ex) 값 + ㅏ = 갑사
                if (key_idx != -1)
                {
                    string newjong = Jcombo[key_idx];  //ㅂㅅ
                    char newjong1 = newjong[0]; //ㅂ
                    char newjong2 = newjong[1]; //ㅅ

                    //갑
                    cho_idx = getIndexinArray<char>(chosung_index, chosung);
                    joong_idx = getIndexinArray<char>(joongsung_index, joongsung);
                    jong_idx = getIndexinArray<char>(jongsung_index, newjong1);

                    char newCh = (char)(((cho_idx * 21) + joong_idx) * 28 + jong_idx + 44032);
                    str_uni += newCh;
                    //사
                    cho_idx = getIndexinArray<char>(chosung_index, newjong2);
                    joong_idx = getIndexinArray<char>(joongsung_index, chKorInput);
                    newCh = (char)(((cho_idx * 21) + joong_idx) * 28 + 44032);
                    str_uni += newCh;

                    inputfiled.text = inputfiled.text.Remove(inputfiled.text.Length - 1);
                    inputfiled.text = inputfiled.text.Insert(inputfiled.text.Length, str_uni);
                }
                else //종성이 Combo NO  ex) 강 + ㅑ = 가야
                {
                    //가
                    cho_idx = getIndexinArray<char>(chosung_index, chosung);
                    joong_idx = getIndexinArray<char>(joongsung_index, joongsung);

                    char newCh = (char)(((cho_idx * 21) + joong_idx) * 28 + 44032);
                    str_uni += newCh;

                    cho_idx = getIndexinArray<char>(chosung_index, jongsung);
                    joong_idx = getIndexinArray<char>(joongsung_index, chKorInput);
                    newCh = (char)(((cho_idx * 21) + joong_idx) * 28 + 44032);
                    str_uni += newCh;

                    inputfiled.text = inputfiled.text.Remove(inputfiled.text.Length - 1);
                    inputfiled.text = inputfiled.text.Insert(inputfiled.text.Length, str_uni);
                }
            }
            else if (jongsung == ' ' && JM == 'J')//7.이전 글자가 종성이 없는 경우 + 자음   ex) 가 + ㅇ = 강 , 가 + ㅉ = 가ㅉ
            {
                key_idx = getIndexinArray<char>(jongsung_index, chKorInput);

                //입력한 자음이 받침이 될 수 있는 경우
                if (key_idx != -1)
                {
                    cho_idx = getIndexinArray<char>(chosung_index, chosung);
                    joong_idx = getIndexinArray<char>(joongsung_index, joongsung);
                    jong_idx = getIndexinArray<char>(jongsung_index, chKorInput);

                    char newCh = (char)(((cho_idx * 21) + joong_idx) * 28 + jong_idx + 44032);
                    str_uni += newCh;
                    inputfiled.text = inputfiled.text.Remove(inputfiled.text.Length - 1);
                    inputfiled.text = inputfiled.text.Insert(inputfiled.text.Length, str_uni);
                }
                else//받침이 될 수 없는 경우
                {
                    str_uni += chKorInput;
                    inputfiled.text = inputfiled.text.Insert(inputfiled.text.Length, str_uni);
                }
            }
            else if (jongsung == ' ' && JM == 'M')//8.이전 글자가 종성이 없는 경우 + 모음 
            {
                string newMo = string.Empty;
                newMo += joongsung;
                newMo += chKorInput;

                key_idx = getIndexinArray<string>(Mcombo, newMo);

                //콤보인경우 ex. 구 + ㅣ = 귀
                if (key_idx != -1)
                {
                    char mo = Mcombo_index[key_idx];

                    joong_idx = getIndexinArray<char>(joongsung_index, mo);

                    char newCh = (char)(((cho_idx * 21) + joong_idx) * 28 + 44032);
                    str_uni += newCh;
                    inputfiled.text = inputfiled.text.Remove(inputfiled.text.Length - 1);
                    inputfiled.text = inputfiled.text.Insert(inputfiled.text.Length, str_uni);
                }
                else//이전글자 모음(중성) + 친글자(모음) 콤보 아닌경우 ex.구 + ㅏ = 구ㅏ
                {
                    str_uni += chKorInput;
                    inputfiled.text = inputfiled.text.Insert(inputfiled.text.Length, str_uni);
                }
            }
        }
    }

    char isJaOrMo(char uni)
    {
        if (uni >= 12593 && uni <= 12622)
        {
            return 'J';
        }
        else if (uni >= 12623 && uni <= 12643)
        {
            return 'M';
        }
        else
        {
            return ' ';
        }
    }

    bool isKorean(char uni)
    {
        if ((uni >= 12593 && uni <= 12622) || (uni >= 12623 && uni <= 12643) || uni >= 0xAC00 && uni <= 0xD7AF)
            return true;
        else
            return false;
    }

    public static int getIndexinArray<T>(T[] arr, T value)
    {
        int index = -1;

        for (int i = 0; i < arr.Length; ++i)
        {
            if (arr[i].Equals(value))
            {
                index = i;
                break;
            }
        }

        return index;
    }

    //InputKey_Backspace
    public void OnBackspaceClicked()
    {
        if (korField.text.Length <= 0) return;
        korField.text = korField.text.Substring(0, korField.text.Length - 1);
        caretPosition--;
    }

    //ShiftKey Change
    public void OnShiftClicked()
    {
        if (isSymbolAct)
            OnSymbolClicked();

        isShift = !isShift;

        if (isShift)
        {
            goShiftKey.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 255);
        }
        else
        {
            goShiftKey.transform.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255);
        }

        if (isEngOn)
        {
            if (isShift)
            {
                for (int i = 0; i < LangKey.Length; ++i)
                {
                    LangKey[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = EngShiftKey[i];
                }
            }
            else
            {
                for (int i = 0; i < LangKey.Length; ++i)
                {
                    LangKey[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = EngKey[i];
                }
            }
        }
        else
        {
            if (isShift)
            {
                for (int i = 0; i < LangKey.Length; ++i)
                {
                    LangKey[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = korShiftKey[i];
                }
            }
            else
            {
                for (int i = 0; i < LangKey.Length; ++i)
                {
                    LangKey[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = korKey[i];
                }
            }
        }
    }

    //korean/EnglishKey Change
    public void OnKorEngClicked()
    {
        if (isSymbolAct)
            OnSymbolClicked();

        isEngOn = !isEngOn;

        if (isEngOn)
        {
            if (isShift)
            {
                for (int i = 0; i < LangKey.Length; ++i)
                {
                    LangKey[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = EngShiftKey[i];
                    LangKey[i].transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = korShiftKey[i];
                }
            }
            else
            {
                for (int i = 0; i < LangKey.Length; ++i)
                {
                    LangKey[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = EngKey[i];
                    LangKey[i].transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = korKey[i];
                }
            }
        }
        else
        {
            if (isShift)
            {
                for (int i = 0; i < LangKey.Length; ++i)
                {
                    LangKey[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = korShiftKey[i];
                    LangKey[i].transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = EngShiftKey[i];
                }
            }
            else
            {
                for (int i = 0; i < LangKey.Length; ++i)
                {
                    LangKey[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = korKey[i];
                    LangKey[i].transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = EngKey[i];
                }
            }
        }

    }

    //SymbolKey Change
    public void OnSymbolClicked()
    {
        isSymbolAct = !isSymbolAct;

        //두드려보고 확인해볼 것//
        isShift = false;
        goShiftKey.transform.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255);

        if (isSymbolAct)
        {
            for (int i = 0; i < SymbolKey.Length; ++i)
            {
                LangKey[i].GetComponentInChildren<Text>().text = SymbolKey[i];
                LangKey[i].transform.GetChild(1).gameObject.SetActive(false);
                LangKey[i].transform.GetChild(0).GetComponent<LayoutElement>().preferredWidth = 88;
                LangKey[i].transform.GetChild(0).GetComponent<LayoutElement>().preferredHeight = 88;
            }
            for (int i = 0; i < SymbolKey2.Length; ++i)
            {
                LangKey2[i].GetComponentInChildren<Text>().text = SymbolKey2[i];
            }
        }
        else
        {
            for (int i = 0; i < EngShiftKey.Length; ++i)
            {
                LangKey[i].GetComponentInChildren<Text>().text = EngShiftKey[i];
                LangKey[i].transform.GetChild(1).gameObject.SetActive(true);
                LangKey[i].transform.GetChild(0).GetComponent<LayoutElement>().preferredWidth = 44;
                LangKey[i].transform.GetChild(0).GetComponent<LayoutElement>().preferredHeight = 44;
            }
            for (int i = 0; i < NumKey.Length; ++i)
            {
                LangKey2[i].GetComponentInChildren<Text>().text = NumKey[i];
            }
        }
    }

    void Start()
    {
        //대문자로 되어 있는거 소문자로 변환 해준거
        foreach (GameObject key in LangKey)
        {
            key.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text =
                key.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text.ToLower();
        }
    }

    void Update()
    {
    }

}
