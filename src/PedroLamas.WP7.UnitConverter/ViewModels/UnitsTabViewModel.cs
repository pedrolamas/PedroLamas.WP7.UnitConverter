using System;
using PedroLamas.WP7.UnitConverter.Framework;
using PedroLamas.WP7.UnitConverter.Models;

namespace PedroLamas.WP7.UnitConverter.ViewModels
{
    public class UnitsTabViewModel : Screen
    {
        private IUnitType _unitType;
        private IUnit _unitA, _unitB;
        private double _amountA, _amountB;
        private bool _isAlreadyConverting = false;

        #region Properties

        public IUnit[] Units
        {
            get
            {
                return _unitType.Units;
            }
        }

        [SurviveTombstone]
        public IUnit UnitA
        {
            get
            {
                return _unitA;
            }
            set
            {
                if (_unitA == value)
                    return;

                _unitA = value;

                ConvertFromB();

                NotifyOfPropertyChange(() => UnitA);
            }
        }

        [SurviveTombstone]
        public string AmountA
        {
            get
            {
                return _amountA.ToString(); // +" " + _unitA.Symbol;
            }
            set
            {
                double amountA;

                if (double.TryParse(value, out amountA))
                {
                    if (_amountA == amountA)
                        return;

                    _amountA = amountA;

                    ConvertFromA();

                    NotifyOfPropertyChange(() => AmountA);
                }
                //else
                //    throw new ArgumentOutOfRangeException("value", "Invalid value");
            }
        }

        [SurviveTombstone]
        public IUnit UnitB
        {
            get
            {
                return _unitB;
            }
            set
            {
                if (_unitB == value)
                    return;

                _unitB = value;

                ConvertFromB();

                NotifyOfPropertyChange(() => UnitB);
            }
        }

        [SurviveTombstone]
        public string AmountB
        {
            get
            {
                return _amountB.ToString(); // +" " + _unitB.Symbol;
            }
            set
            {
                double amountB;

                if (double.TryParse(value, out amountB))
                {
                    if (_amountB == amountB)
                        return;

                    _amountB = amountB;

                    ConvertFromB();

                    NotifyOfPropertyChange(() => AmountB);
                }
                //else
                //    throw new ArgumentOutOfRangeException("value", "Invalid value");
            }
        }

        #endregion

        public UnitsTabViewModel(IUnitType unitType)
        {
            _unitType = unitType;
            _unitA = unitType.Units[0];
            _unitB = unitType.Units[1];

            ConvertFromA();

            DisplayName = unitType.Name;
        }

        private void ConvertFromA()
        {
            if (_isAlreadyConverting)
                return;

            _isAlreadyConverting = true;

            AmountB = ((_amountA - _unitA.RelativeDisplacement) * (_unitB.RelativeFactor / _unitA.RelativeFactor) + _unitB.RelativeDisplacement).ToString();

            _isAlreadyConverting = false;
        }

        private void ConvertFromB()
        {
            if (_isAlreadyConverting)
                return;

            _isAlreadyConverting = true;

            AmountA = ((_amountB - _unitB.RelativeDisplacement) * (_unitA.RelativeFactor / _unitB.RelativeFactor) + _unitA.RelativeDisplacement).ToString();

            _isAlreadyConverting = false;
        }
    }
}