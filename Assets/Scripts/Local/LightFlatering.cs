using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace local
{
    public class LightFlatering : MonoBehaviour
    {
        [SerializeField] float _flateringMaxTime;
        [SerializeField] float _flateringMinTime;
        [SerializeField] float _maxAlpha;
        [SerializeField] float _minAlpha;

        SpriteRenderer _backGroundRender;
        float _flateringTime;
        float _timer;
        float _value;
        bool _onIncrease;

        private void Awake()
        {
            _backGroundRender = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _onIncrease = true;
            _timer = 0f;
            _value = 1f;
            _flateringTime = Random.Range(_flateringMinTime, _flateringMaxTime);
        }

        // Update is called once per frame
        void Update()
        {
            if(_timer <= _flateringTime)
            {
                if(_onIncrease)
                    _value = Mathf.Lerp(_minAlpha, _maxAlpha, _timer/_flateringTime);
                else
                    _value = Mathf.Lerp(_maxAlpha, _minAlpha, _timer / _flateringTime);
                
                _timer += Time.deltaTime;
            }
            else // Seteamos variables y recalculamos rangos
            {
                _timer = 0;
                // Nuevo sentido 
                if (_onIncrease) _onIncrease = false;
                else _onIncrease = true;
                // Nuevo rango de tiempo
                _flateringTime = Random.Range(_flateringMinTime, _flateringMaxTime);
            }
            _backGroundRender.color = new Vector4(_backGroundRender.color.r, _backGroundRender.color.g, _backGroundRender.color.b, _value);
        }
    }
}

