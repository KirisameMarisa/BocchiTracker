using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CameraController : MonoBehaviour
{
    // WASD�F�O�㍶�E�̈ړ�
    // QE�F�㏸�E�~��
    // ���h���b�O�F���_�̈ړ�

    //�J�����̈ړ���
    [SerializeField, Range(0.1f, 20.0f)]
    private float _positionStep = 5.0f;

    //�}�E�X���x
    [SerializeField, Range(30.0f, 300.0f)]
    private float _mouseSensitive = 90.0f;

    //�J������transform  
    private Transform _camTransform;

    //�}�E�X�̎n�_ 
    private Vector3 _startMousePos;

    //�J������]�̎n�_���
    private Vector3 _presentCamRotation;
    private Vector3 _presentCamPos;


    void Start()
    {
        _camTransform = this.gameObject.transform;
    }

    void Update()
    {
        CameraRotationMouseControl(); //�J�����̉�] �}�E�X
        CameraPositionKeyControl();   //�J�����̃��[�J���ړ� �L�[
    }


    //�J�����̉�] �}�E�X
    private void CameraRotationMouseControl()
    {
        /* �}�E�X���N���b�N���ꂽ�Ƃ� */
        if (Input.GetMouseButtonDown(0))
        {
            _startMousePos = Input.mousePosition;
            _presentCamRotation.x = _camTransform.transform.eulerAngles.x;
            _presentCamRotation.y = _camTransform.transform.eulerAngles.y;
        }

        /* �}�E�X���N���b�N����Ă���� */
        if (Input.GetMouseButton(0))
        {
            //(�ړ��J�n���W - �}�E�X�̌��ݍ��W) / �𑜓x �Ő��K��
            float x = (_startMousePos.x - Input.mousePosition.x) / Screen.width;
            float y = (_startMousePos.y - Input.mousePosition.y) / Screen.height;

            //��]�J�n�p�x �{ �}�E�X�̕ω��� * �}�E�X���x
            float eulerX = _presentCamRotation.x + y * _mouseSensitive;
            float eulerY = _presentCamRotation.y + x * _mouseSensitive;

            _camTransform.rotation = Quaternion.Euler(eulerX, eulerY, 0);
        }
    }

    //�J�����̃��[�J���ړ� �L�[
    private void CameraPositionKeyControl()
    {
        Vector3 campos = _camTransform.position;

        if (Input.GetKey(KeyCode.D)) { campos += _camTransform.right * Time.deltaTime * _positionStep; }
        if (Input.GetKey(KeyCode.A)) { campos -= _camTransform.right * Time.deltaTime * _positionStep; }
        if (Input.GetKey(KeyCode.E)) { campos += _camTransform.up * Time.deltaTime * _positionStep; }
        if (Input.GetKey(KeyCode.Q)) { campos -= _camTransform.up * Time.deltaTime * _positionStep; }
        if (Input.GetKey(KeyCode.W)) { campos += _camTransform.forward * Time.deltaTime * _positionStep; }
        if (Input.GetKey(KeyCode.S)) { campos -= _camTransform.forward * Time.deltaTime * _positionStep; }

        _camTransform.position = campos;
    }
}