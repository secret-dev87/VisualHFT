﻿using System.Collections.ObjectModel;
using VisualHFT.Commons.Pools;
using VisualHFT.Helpers;
using VisualHFT.Studies;

namespace VisualHFT.Model
{
    public partial class OrderBook : ICloneable, IDisposable
    {
        private bool _disposed = false; // to track whether the object has been disposed

        protected CachedCollection<BookItem> _Asks;
        protected CachedCollection<BookItem> _Bids;

        private CachedCollection<BookItem> _Cummulative_Bids;
        private CachedCollection<BookItem> _Cummulative_Asks;

        private object LOCK_OBJECT = new object();

        private string _Symbol;
        private int _DecimalPlaces;
        private double _SymbolMultiplier;
        private int _ProviderID;
        private string _ProviderName;
        private eSESSIONSTATUS _providerStatus;
        private double _MidPrice = 0;
        private double _Spread = 0;
        private BookItem _bidTOP = null;
        private BookItem _askTOP = null;
        OrderFlowAnalysis lobMetrics = new OrderFlowAnalysis();
        private readonly Commons.Pools.ObjectPool<BookItem> _cummObjectPool = new Commons.Pools.ObjectPool<BookItem>();

        public OrderBook() //emtpy constructor for JSON deserialization
        {
            _Cummulative_Asks = new CachedCollection<BookItem>();
            _Cummulative_Bids = new CachedCollection<BookItem>();
            _Bids = new CachedCollection<BookItem>();
            _Asks = new CachedCollection<BookItem>();
        }
        public OrderBook(string symbol, int decimalPlaces)
        {
            _Cummulative_Asks = new CachedCollection<BookItem>();
            _Cummulative_Bids = new CachedCollection<BookItem>();
            _Bids = new CachedCollection<BookItem>();
            _Asks = new CachedCollection<BookItem>();

            _Symbol = symbol;
            _DecimalPlaces = decimalPlaces;
        }
        ~OrderBook()
        {
            Dispose(false);
        }
        public void GetAddDeleteUpdate(ref ObservableCollection<BookItem> inputExisting, ReadOnlyCollection<BookItem> listToMatch)
        {
            ReadOnlyCollection<BookItem> inputNew = listToMatch;
            List<BookItem> outAdds;
            List<BookItem> outUpdates;
            List<BookItem> outRemoves;

            var existingSet = inputExisting; // new HashSet<BookItem>(inputExisting);
            var newSet = inputNew; // new HashSet<BookItem>(inputNew);

            outRemoves = inputExisting.Where(e => !newSet.Contains(e)).ToList();
            outUpdates = inputNew.Where(e => existingSet.Contains(e) && e.Size != existingSet.FirstOrDefault(i => i.Equals(e)).Size).ToList();
            outAdds = inputNew.Where(e => !existingSet.Contains(e)).ToList();

            foreach (var b in outRemoves)
                inputExisting.Remove(b);
            foreach (var b in outUpdates)
            {
                var itemToUpd = inputExisting.Where(x => x.Price == b.Price).FirstOrDefault();
                if (itemToUpd != null)
                {
                    itemToUpd.Size = b.Size;
                    itemToUpd.ActiveSize = b.ActiveSize;
                    itemToUpd.LocalTimeStamp = b.LocalTimeStamp;
                    itemToUpd.ServerTimeStamp = b.ServerTimeStamp;
                }
            }
            foreach (var b in outAdds)
                inputExisting.Add(b);
        }
        private void CalculateMetrics()
        {
            lobMetrics.LoadData(_Asks, _Bids);
            this.ImbalanceValue = lobMetrics.Calculate_OrderImbalance();
        }
        public void Clear()
        {
            lock (LOCK_OBJECT)
            {
                _Cummulative_Asks?.Clear();
                _Cummulative_Bids?.Clear();
                _Bids?.Clear();
                _Asks?.Clear();
            }
        }
        public bool LoadData()
        {
            return LoadData(this.Asks, this.Bids);
        }
        public bool LoadData(IEnumerable<BookItem> asks, IEnumerable<BookItem> bids)
        {
            bool ret = true;
            lock (LOCK_OBJECT)
            {
                #region Bids
                if (bids != null)
                {
                    _Bids.Update(bids
                        .Where(x => x != null && x.Price.HasValue && x.Size.HasValue)
                        .OrderByDescending(x => x.Price.Value)
                    );
                }

                foreach (var item in _Cummulative_Bids)
                    _cummObjectPool.Return(item);
                _Cummulative_Bids.Clear();

                double cumSize = 0;
                foreach (var o in _Bids)
                {
                    cumSize += o.Size.Value;
                    var _item = _cummObjectPool.Get();
                    _item.Price = o.Price;
                    _item.Size = cumSize;
                    _item.IsBid = true;
                    _Cummulative_Bids.Add(_item);
                }
                #endregion

                #region Asks
                if (asks != null)
                {
                    _Asks.Update(asks
                        .Where(x => x != null && x.Price.HasValue && x.Size.HasValue)
                        .OrderBy(x => x.Price.Value)
                    );
                }

                foreach (var item in _Cummulative_Asks)
                    _cummObjectPool.Return(item);
                _Cummulative_Asks.Clear();
                cumSize = 0;
                foreach (var o in _Asks)
                {
                    cumSize += o.Size.Value;
                    var _item = _cummObjectPool.Get();
                    _item.Price = o.Price;
                    _item.Size = cumSize;
                    _item.IsBid = false;
                    _Cummulative_Asks.Add(_item);
                }
                #endregion

                _bidTOP = _Bids.FirstOrDefault();
                _askTOP = _Asks.FirstOrDefault();
                if (_bidTOP != null && _bidTOP.Price.HasValue && _askTOP != null && _askTOP.Price.HasValue)
                {
                    _MidPrice = (_bidTOP.Price.Value + _askTOP.Price.Value) / 2;
                    _Spread = _askTOP.Price.Value - _bidTOP.Price.Value;
                }

                CalculateMetrics();
            }
            return ret;
        }
        public BookItem GetTOB(bool isBid)
        {
            lock (LOCK_OBJECT)
            {
                if (isBid)
                    return _bidTOP;
                else
                    return _askTOP;
            }
        }
        public double GetMaxOrderSize()
        {
            double _maxOrderSize = 0;

            lock (LOCK_OBJECT)
            {
                if (_Bids != null)
                    _maxOrderSize = _Bids.Where(x => x.Size.HasValue).DefaultIfEmpty(new BookItem()).Max(x => x.Size.Value);
                if (_Asks != null)
                    _maxOrderSize = Math.Max(_maxOrderSize, _Asks.Where(x => x.Size.HasValue).DefaultIfEmpty(new BookItem()).Max(x => x.Size.Value));
            }
            return _maxOrderSize;
        }
        public Tuple<double, double> GetMinMaxSizes()
        {
            List<BookItem> allOrders = new List<BookItem>();

            lock (LOCK_OBJECT)
            {
                if (_Bids != null)
                    allOrders.AddRange(_Bids.Where(x => x.Size.HasValue).ToList());
                if (_Asks != null)
                    allOrders.AddRange(_Asks.Where(x => x.Size.HasValue).ToList());
            }
            //AVOID OUTLIERS IN SIZES (when data is invalid)
            double firstQuantile = allOrders.Select(x => x.Size.Value).Quantile(0.25);
            double thirdQuantile = allOrders.Select(x => x.Size.Value).Quantile(0.75);
            double iqr = thirdQuantile - firstQuantile;
            double lowerBand = firstQuantile - 1.5 * iqr;
            double upperBound = thirdQuantile + 1.5 * iqr;

            double minOrderSize = allOrders.Where(x => x.Size >= lowerBand).Min(x => x.Size.Value);
            double maxOrderSize = allOrders.Where(x => x.Size <= upperBound).Max(x => x.Size.Value);

            return Tuple.Create(minOrderSize, maxOrderSize);
        }


        private readonly ObjectPool<OrderBook> orderBookPool = new ObjectPool<OrderBook>();

        public object Clone()
        {
            var clone = orderBookPool.Get();  // Get a pooled instance instead of creating a new one
            clone.DecimalPlaces = DecimalPlaces;
            clone.ProviderID = ProviderID;
            clone.ProviderName = ProviderName;
            clone.Symbol = Symbol;
            clone.SymbolMultiplier = SymbolMultiplier;
            clone.ImbalanceValue = ImbalanceValue;
            clone.ProviderStatus = ProviderStatus;

            clone.LoadData(Asks, Bids);
            return clone;
        }
        public void CopyTo(OrderBook target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            target.DecimalPlaces = this.DecimalPlaces;
            target.ProviderID = this.ProviderID;
            target.ProviderName = this.ProviderName;
            target.Symbol = this.Symbol;
            target.SymbolMultiplier = this.SymbolMultiplier;
            target.ImbalanceValue = this.ImbalanceValue;
            target.ProviderStatus = this.ProviderStatus;

            target.LoadData(this.Asks, this.Bids);
        }

        public ReadOnlyCollection<BookItem> Asks
        {
            get => _Asks.ToList().AsReadOnly();
            set => _Asks.Update(value); //do not remove setter: it is used to auto parse json
        }
        public ReadOnlyCollection<BookItem> Bids
        {
            get => _Bids.ToList().AsReadOnly();
            set => _Bids.Update(value); //do not remove setter: it is used to auto parse json
        }
        public ReadOnlyCollection<BookItem> BidCummulative
        {
            get { lock (LOCK_OBJECT) return _Cummulative_Bids.ToList().AsReadOnly(); }
        }
        public ReadOnlyCollection<BookItem> AskCummulative
        {
            get { lock (LOCK_OBJECT) return _Cummulative_Asks.ToList().AsReadOnly(); }
        }
        public void PrintLOB(bool isBid)
        {
            lock (LOCK_OBJECT)
            {
                int _level = 0;
                foreach(var item in isBid? _Bids: _Asks) {
                    Console.WriteLine($"{_level} - {item.FormattedPrice} [{item.Size}]");
                    _level++;
                }
            }
        }
        public string Symbol { get => _Symbol; set => _Symbol = value; }
        public int DecimalPlaces { get => _DecimalPlaces; set => _DecimalPlaces = value; }
        public double SymbolMultiplier { get => _SymbolMultiplier; set => _SymbolMultiplier = value; }
        public int ProviderID { get => _ProviderID; set => _ProviderID = value; }
        public string ProviderName { get => _ProviderName; set => _ProviderName = value; }
        public eSESSIONSTATUS ProviderStatus { get => _providerStatus; set => _providerStatus = value; }
        public double ImbalanceValue { get; set; }
        public double MidPrice { get => _MidPrice; }
        public double Spread { get => _Spread; }





        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _Cummulative_Asks?.Clear();
                    _Cummulative_Bids?.Clear();
                    _Bids?.Clear();
                    _Asks?.Clear();


                    _Cummulative_Asks = null;
                    _Cummulative_Bids = null;
                    _Bids = null;
                    _Asks = null;

                    _bidTOP = null;
                    _askTOP = null;

                    orderBookPool.Return(this);  // Return to the pool
                }
                _disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
