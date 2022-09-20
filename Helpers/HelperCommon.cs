﻿using VisualHFT.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VisualHFT.Helpers
{
    public class HelperCommon
    {
        public static DateTime GetCurrentSessionIni(DateTime? currentSessionDate)
        {
            currentSessionDate = currentSessionDate.ToDate().AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);
            if (!currentSessionDate.HasValue)
                currentSessionDate = DateTime.Now;
            DateTime DateAt00 = currentSessionDate.ToDate();
            DateTime DateAt500PM = DateAt00.AddHours(17);
            DateTime DateAt530PM = DateAt00.AddHours(17).AddMinutes(30);
            if (currentSessionDate > DateAt500PM && currentSessionDate < DateAt00.AddDays(1))
                return DateAt530PM;
            else if (currentSessionDate > DateAt00 && currentSessionDate < DateAt500PM)
                return DateAt530PM.AddDays(-1);
            else
                return DateTime.Now;
        }
        public static DateTime GetCurrentSessionEnd(DateTime? currentSessionDate)
        {
            currentSessionDate = currentSessionDate.ToDate().AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);
            if (!currentSessionDate.HasValue)
                currentSessionDate = DateTime.Now;
            DateTime DateAt00 = currentSessionDate.ToDate();
            DateTime DateAt500PM = DateAt00.AddHours(17);
            DateTime DateAt530PM = DateAt00.AddHours(17).AddMinutes(30);
            if (currentSessionDate > DateAt500PM && currentSessionDate < DateAt00.AddDays(1))
                return DateAt500PM.AddDays(1);
            else if (currentSessionDate > DateAt00 && currentSessionDate < DateAt500PM)
                return DateAt500PM;
            else
                return DateTime.Now;
        }
        public static int TimerMillisecondsToGetVariables = 1000 * 10; //10 seconds
        
        public static ObservableCollection<string> ALLSYMBOLS = new ObservableCollection<string>();
        public static HelperProvider PROVIDERS = new HelperProvider();
        public static HelperOrderBook LIMITORDERBOOK = new HelperOrderBook();
        public static HelperPosition CLOSEDPOSITIONS = new HelperPosition(ePOSITION_LOADING_TYPE.DATABASE);
        public static HelperPosition OPENPOSITIONS = new HelperPosition(ePOSITION_LOADING_TYPE.WEBSOCKETS);
        public static HelperExposure EXPOSURES = new HelperExposure();
        public static HelperActiveOrder ACTIVEORDERS = new HelperActiveOrder();
        public static HelperStrategy ACTIVESTRATEGIES = new HelperStrategy();
        public static HelperStrategyParams STRATEGYPARAMS = new HelperStrategyParams();
        public static Func<string, string, bool> GetPopup()
        {
            return (Func<string, string, bool>)((msg, capt) => MessageBox.Show(msg, capt, MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK);
        }
        public static Func<string, string, bool> GetConfirmPopup()
        {
            return (Func<string, string, bool>)((msg, capt) => MessageBox.Show(msg, capt, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes);
        }
        public static Func<string, string, bool> GetValidationPopup()
        {
            return (Func<string, string, bool>)((msg, capt) => MessageBox.Show(msg, capt, MessageBoxButton.OK, MessageBoxImage.Asterisk) == MessageBoxResult.Yes);
        }
        public static Func<string, string, bool> GetErrorPopup()
        {
            return (Func<string, string, bool>)((msg, capt) => MessageBox.Show(msg, capt, MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.Yes);
        }


        public static Dictionary<string, Func<string, string, bool>> GLOBAL_DIALOGS = new Dictionary<string, Func<string, string, bool>>()
                {
                    {"popup", GetPopup() },
                    {"confirm", GetConfirmPopup() },
                    {"validation", GetValidationPopup() },
                    {"error", GetErrorPopup() }
                };        
        public static double GetPipValueInBaseCurrency(bool getBid, string currencySymbol, int symbolMultiplier, decimal pipsValue, decimal size, string baseCurrency = "USD")
        {
            string ccy1 = currencySymbol.Split('/')[0];
            string ccy2 = currencySymbol.Split('/')[1];
            double rate = 0;
            if (ccy2 == baseCurrency)
                rate = 1;
            else
            {
                //for example-> USD/GBP GBP=X
                if (baseCurrency == "USD")
                    rate = GetCurrencyRate(ccy2 + "=X", getBid);
                else
                    rate = GetCurrencyRate(baseCurrency + "/" + ccy2, getBid);
            }
            double dRet = (double)(pipsValue * size) / (symbolMultiplier * rate);
            return dRet;
        }
        public static double GetCurrencyRate(string currencySymbol, bool getBid)
        {
            var stock = HelperYahoo.GetStock(currencySymbol);
            if (stock == null)
                return 0;
            else
                return (getBid ? stock.Bid : stock.Ask);
        }
        public static string GetKiloFormatter(int num)
        {
            return GetKiloFormatter((double)num);
        }
        public static string GetKiloFormatter(decimal num)
        {
            return GetKiloFormatter((double)num);
        }
        public static string GetKiloFormatter(double num)
        {
            if (num < 500)
                return num.ToString();
            if (num < 10000)
                return num.ToString("N0");


            if (num >= 100000000)
                return (num / 1000000D).ToString("0.#M");
            if (num >= 1000000)
                return (num / 1000000D).ToString("0.##M");
            if (num >= 100000)
                return (num / 1000D).ToString("0.#k");
            if (num >= 10000)
                return (num / 1000D).ToString("0.##k");
            return num.ToString("#,0");
        }
        public static string GetKiloFormatterTime(double milliseconds)
        {
            double num = milliseconds;

            if (num >= 1000 * 60 * 60.0)
                return (num / (60.0 * 60.0 * 1000D)).ToString("0.0 hs");
            if (num >= 1000 * 60.0)
                return (num / (60.0 * 1000D)).ToString("0.0 min");
            if (num >= 1000)
                return (num / 1000D).ToString("0.0# sec");

            return num.ToString("#,0 ms");
        }

    }
}

