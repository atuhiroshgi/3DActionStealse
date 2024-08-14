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
                explainText.text = "�E�X�g�b�N��1����ăL�����������Ă�������ɔ�ׂ�\n\n�E�v���C�Ɏ��M������Q�[���Ɋ���Ă���N�ɂ������߁I";
                break;

            case 1:
                explainText.text = "�E�X�g�b�N��2����ē���������\n\n�E���������͐l�ԂɌ����Ă��x���x�����܂炸�A�ړ����x��W�����v�̍������オ��\n\n�E�������ڂ����ӂȌN�ɂ������߁I";
                break;

            case 2:
                explainText.text = "�E�X�g�b�N��5����ċ��剻����\n\n�E���剻�������΂�������Ɛl�Ԃ͋C�₷��\n\n�E�͂ŃS�����������N�ɂ������߁I";
                break;
        }
    }
}
