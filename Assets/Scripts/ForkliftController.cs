using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public enum Axel
{
    Front,
    Rear
}

[Serializable]
public struct Wheel
{
    public GameObject model;
    public WheelCollider collider;
    public Axel axel;
}


public class ForkliftController : MonoBehaviour
{
    public GameObject myBox;
    private bool _isMovingForward, _isMovingBackward, _isBrake, _isRotating, _moveUp, _moveDown;
    private float _fuelValue, _inputX;
    private int _forwardSpeed;
    private Rigidbody _rb;
    [SerializeField] GameObject fuelPrefab;
    [SerializeField] private Image fuelBar;
    [SerializeField] private Transform wheelImage;
    [SerializeField] private Transform forkPlatform;
    [SerializeField] private float turnSensitivity;
    [SerializeField] private Vector3 _centerOfMass;
    [SerializeField] private List<Wheel> wheels;

    private void Start()
    {
        _forwardSpeed = 22500;
        _fuelValue = 100;
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = _centerOfMass;
    }

    private void Update()
    {
        WheelsBehaviour();
        GetInputs();
        ForkPlatformAction();
    }

    private void LateUpdate()
    {
        Movement();
        Turn();
    }


    private void Movement()
    {
        if (_isMovingForward)
        {
            Brake(0);
            Move(1);
            UsingFuel(5);
        }

        if (_isBrake)
        {
            Brake(1);
            Move(0);
        }

        else if (_isMovingBackward)
        {
            Brake(0);
            Move(-1);
            UsingFuel(5);
        }
    }

    private void ForkPlatformAction()
    {
        if (_moveUp && forkPlatform.transform.localPosition.y <= 0.01f)
        {
            forkPlatform.transform.position += Vector3.up * Time.deltaTime * 0.3f;
        }

        if (_moveDown && forkPlatform.transform.localPosition.y >= 0)
        {
            forkPlatform.transform.position -= Vector3.up * Time.deltaTime * 0.3f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Fuel")
        {
            SetFuelValue(100);
            StartCoroutine(ReplaceFuel(other.transform.position, other.transform.parent));
            Destroy(other.gameObject);

        }

        if (other.tag == "Floor")
        {
            transform.parent = other.transform.parent;

            if (myBox != null)
                myBox.transform.parent = other.transform.parent;
        }

        if (other.tag == "Pendulum" || other.tag == "Death")
        {
            GameManager.Instance.Fail();
        }
    }

    #region Button Events


    #region GasPedal
    public void MovingForward(bool isMovingForward)
    {
        _isMovingForward = isMovingForward;
    }
    public void MovingBackward(bool isMovingBackward)
    {
        _isMovingBackward = isMovingBackward;
    }
    #endregion

    #region BrakePedal
    public void Brake(bool isBrake)
    {
        _isBrake = isBrake;
    }
    #endregion

    #region ForkPlatform
    public void PlatformUpState(bool moveUp)
    {
        _moveUp = moveUp;
    }
    public void PlatformDownState(bool moveDown)
    {
        _moveDown = moveDown;
    }
    #endregion


    #endregion

    #region Move Actions
    private void Move(float speedMultiplier)
    {
        foreach (var wheel in wheels)
        {
            wheel.collider.motorTorque = speedMultiplier * _forwardSpeed * Time.deltaTime;
        }
    }

    private void Brake(float speedMultiplier)
    {
        foreach (var wheel in wheels)
        {
            wheel.collider.brakeTorque = speedMultiplier * _forwardSpeed * Time.deltaTime;
        }
    }

    private void Turn()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = Mathf.Clamp(_inputX, -45f, 45f);
                wheel.collider.steerAngle = Mathf.Lerp(wheel.collider.steerAngle, _steerAngle, 0.5f);
                wheelImage.localEulerAngles = new Vector3(0, 0, -_steerAngle);
            }
        }
    }

    #endregion

    #region Wheel Actions
    public void WheelSituation(bool isRotating)
    {
        _isRotating = isRotating;
    }
    private void GetInputs()
    {
        if(_isRotating)
        {
            if(Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if(touch.phase == TouchPhase.Moved)
                {
                    _inputX += touch.deltaPosition.x * Time.deltaTime * turnSensitivity;
                }
            }
        }
    }

    private void WheelsBehaviour()
    {
        foreach (var wheel in wheels)
        {
            Quaternion _rot;
            Vector3 _pos;
            wheel.collider.GetWorldPose(out _pos, out _rot);
            wheel.model.transform.position = _pos;
            wheel.model.transform.rotation = _rot;
        }
    }

    #endregion

    #region FUEL
    private void UsingFuel(float val)
    {
        _fuelValue -= val * Time.deltaTime;
        fuelBar.fillAmount = _fuelValue / 100;

        if (_fuelValue <= 0)
        {
            _fuelValue = 0;
            GameManager.Instance.Fail();
        }
    }
    private void SetFuelValue(float val)
    {
        _fuelValue = val;
        fuelBar.fillAmount = _fuelValue / 100;
    }

    IEnumerator ReplaceFuel(Vector3 pos, Transform targetParent)
    {
        yield return new WaitForSeconds(10);
        GameObject newFuel = Instantiate(fuelPrefab, targetParent);
        newFuel.transform.localScale = Vector3.zero;
        newFuel.transform.DOScale(Vector3.one * 15, 0.5f);
        newFuel.transform.position = pos;
    }

    #endregion

}