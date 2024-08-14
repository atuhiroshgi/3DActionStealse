using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplainSkill : MonoBehaviour
{
    [SerializeField] private Text explainText;
    [SerializeField] private GameObject UpArrow;
    [SerializeField] private GameObject DownArrow;

    private enum State
    {
        NORMAL,
        HIDDEN,
        HUGE,
    }

    private State currentState = State.NORMAL;

    private void Update()
    {

        if (GameManager.Instance.GetGhostMoving())
        {
            UpArrow.SetActive(false);
            DownArrow.SetActive(false);
            explainText.text = null;
            return;
        }

        switch (GameManager.Instance.GetSelectedIndex())
        {
            case 0:
                explainText.text = "�E�X�g�b�N��1����ău�[�X�g�ł���\n\n�E�u�[�X�g����Ƒ���s�\�ɂȂ�A���΂��������Ă�������ɃW�����v����\n\n�E�v���C�Ɏ��M������Q�[���Ɋ���Ă���N�ɂ������߁I";
                UpArrow.SetActive(false);
                DownArrow.SetActive(true);
                break;

            case 1:
                explainText.text = "�E�X�g�b�N��2����ē���������\n\n�E���������͐l�ԂɌ����Ă��x���x�����܂炸�A�ړ����x��W�����v�̍������オ��\n\n�E�������ڂ����ӂȌN�ɂ������߁I";
                UpArrow.SetActive(true);
                DownArrow.SetActive(true);
                break;

            case 2:
                explainText.text = "�E�X�g�b�N��5����ċ��剻����\n\n�E���剻�������΂�������Ɛl�Ԃ͋C�₷��\n\n�E�͂ŃS�����������N�ɂ������߁I";
                UpArrow.SetActive(true);
                DownArrow.SetActive(false);
                break;
        }
    }
}
