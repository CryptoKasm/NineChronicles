using System;
using System.Collections.Generic;
using System.Text;
using Nekoyume.Action;
using Nekoyume.Helper;
using Nekoyume.L10n;
using Nekoyume.State;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Nekoyume.UI
{
    public class BlockTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI remainTime = null;

        [SerializeField] private Slider remainTimeSlider = null;

        private readonly List<IDisposable> _disposablesFromOnEnable = new List<IDisposable>();

        private long _expiredTime;

        private void Awake()
        {
            remainTimeSlider.OnValueChangedAsObservable().Subscribe(OnSliderChange)
                .AddTo(gameObject);
            remainTimeSlider.maxValue = Sell.ExpiredBlockIndex;
            remainTimeSlider.value = 0;
        }

        private void OnEnable()
        {
            Game.Game.instance.Agent.BlockIndexSubject.Subscribe(SetBlockIndex)
                .AddTo(_disposablesFromOnEnable);
        }

        private void OnDisable()
        {
            _disposablesFromOnEnable.DisposeAllAndClear();
        }

        private void SetBlockIndex(long blockIndex)
        {
            remainTimeSlider.value = _expiredTime - blockIndex;
        }

        private void OnSliderChange(float value)
        {
            var time = Util.GetBlockToTime((int) value);
            remainTime.text = string.Format(L10nManager.Localize("UI_BLOCK_TIMER"), value, time);
        }

        public void UpdateTimer(long expiredTime)
        {
            _expiredTime = expiredTime;
            remainTimeSlider.value = _expiredTime - Game.Game.instance.Agent.BlockIndex;
        }
    }
}
