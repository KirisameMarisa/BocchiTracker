using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CameraController : MonoBehaviour
{
    // WASD：前後左右の移動
    // QE：上昇・降下
    // 左ドラッグ：視点の移動

    //カメラの移動量
    [SerializeField, Range(0.1f, 20.0f)]
    private float _positionStep = 5.0f;

    //マウス感度
    [SerializeField, Range(30.0f, 300.0f)]
    private float _mouseSensitive = 90.0f;

    //カメラのtransform  
    private Transform _camTransform;

    //マウスの始点 
    private Vector3 _startMousePos;

    //カメラ回転の始点情報
    private Vector3 _presentCamRotation;
    private Vector3 _presentCamPos;


    void Start()
    {
        _camTransform = this.gameObject.transform;
    }

    void Update()
    {
        CameraRotationMouseControl(); //カメラの回転 マウス
        CameraPositionKeyControl();   //カメラのローカル移動 キー
    }

    //カメラの回転 マウス
    private void CameraRotationMouseControl()
    {
        /* マウスがクリックされたとき */
        if (Input.GetMouseButtonDown(0))
        {
            _startMousePos = Input.mousePosition;
            _presentCamRotation.x = _camTransform.transform.eulerAngles.x;
            _presentCamRotation.y = _camTransform.transform.eulerAngles.y;
        }

        /* マウスがクリックされている間 */
        if (Input.GetMouseButton(0))
        {
            //(移動開始座標 - マウスの現在座標) / 解像度 で正規化
            float x = (_startMousePos.x - Input.mousePosition.x) / Screen.width;
            float y = (_startMousePos.y - Input.mousePosition.y) / Screen.height;

            //回転開始角度 ＋ マウスの変化量 * マウス感度
            float eulerX = _presentCamRotation.x + (y * _mouseSensitive);
            float eulerY = _presentCamRotation.y + (x * _mouseSensitive);

            _camTransform.rotation = Quaternion.Euler(eulerX, eulerY, 0);
        }
    }

    //カメラのローカル移動 キー
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