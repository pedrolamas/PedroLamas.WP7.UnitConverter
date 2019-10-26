using System.ComponentModel;

namespace PedroLamas.WP7.UnitConverter.Models
{
    public class UnitsConverter : IUnitsConverter, INotifyPropertyChanged
    {
        public const string UnitTypePropertyName = "UnitType";
        public const string UnitAPropertyName = "UnitA";
        public const string UnitBPropertyName = "UnitB";
        public const string AmountAPropertyName = "AmountA";
        public const string AmountBPropertyName = "AmountB";

        private UnitsData _unitsData;
        private IUnitType _unitType;
        private IUnit _unitA, _unitB;
        private double _amountA, _amountB;
        private bool _isAlreadyConverting = false;

        #region Properties

        public IUnitType[] UnitTypes
        {
            get
            {
                return _unitsData.UnitTypes;
            }
        }

        public IUnitType UnitType
        {
            get
            {
                return _unitType;
            }
            set
            {
                if (_unitType == value)
                    return;

                _unitType = value;

                UnitA = _unitType.Units[0];
                UnitB = _unitType.Units[1];

                RaisePropertyChanged(UnitTypePropertyName);
            }
        }

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

                RaisePropertyChanged(UnitAPropertyName);
            }
        }

        public double AmountA
        {
            get
            {
                return _amountA;
            }
            set
            {
                if (_amountA == value)
                    return;

                _amountA = value;

                ConvertFromA();

                RaisePropertyChanged(AmountAPropertyName);
            }
        }

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

                ConvertFromA();

                RaisePropertyChanged(UnitBPropertyName);
            }
        }

        public double AmountB
        {
            get
            {
                return _amountB;
            }
            set
            {
                if (_amountB == value)
                    return;

                _amountB = value;

                ConvertFromB();

                RaisePropertyChanged(AmountBPropertyName);
            }
        }

        #endregion

        public UnitsConverter()
        {
            _unitsData = UnitsData.Deserialize();

            _unitType = _unitsData.UnitTypes[0];
            _unitA = _unitType.Units[0];
            _unitB = _unitType.Units[1];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ConvertFromA()
        {
            if (_isAlreadyConverting)
                return;

            _isAlreadyConverting = true;

            AmountB = (_amountA - _unitA.RelativeDisplacement) * (_unitB.RelativeFactor / _unitA.RelativeFactor) + _unitB.RelativeDisplacement;

            _isAlreadyConverting = false;
        }

        private void ConvertFromB()
        {
            if (_isAlreadyConverting)
                return;

            _isAlreadyConverting = true;

            AmountA = (_amountB - _unitB.RelativeDisplacement) * (_unitA.RelativeFactor / _unitB.RelativeFactor) + _unitA.RelativeDisplacement;

            _isAlreadyConverting = false;
        }
    }
}