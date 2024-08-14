using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplainSkill : MonoBehaviour
{
    [SerializeField] private Text explainText;

    private enum State
    {
        NORMAL,
        HIDDEN,
        HUGE,
    }

    private State currentState = State.NORMAL;

    private void Update()
    {
        switch (GameManager.Instance.GetSelectedIndex())
        {
            case 0:
                explainText.text = "・ストックを1消費してキャラが向いている方向に飛べる\n\n・プレイに自信があるゲームに慣れている君におすすめ！";
                break;

            case 1:
                explainText.text = "・ストックを2消費して透明化する\n\n・透明化中は人間に見られても警戒度が溜まらず、移動速度やジャンプの高さが上がる\n\n・かくれんぼが得意な君におすすめ！";
                break;

            case 2:
                explainText.text = "・ストックを5消費して巨大化する\n\n・巨大化したおばけを見ると人間は気絶する\n\n・力でゴリ押したい君におすすめ！";
                break;
        }
    }
}
