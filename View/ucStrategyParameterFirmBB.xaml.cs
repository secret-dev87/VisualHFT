﻿using System.Windows;
using System.Windows.Controls;

namespace VisualHFT.View
{
    /// <summary>
    /// Interaction logic for ucStrategyParameterFirmBB.xaml
    /// </summary>
    public partial class ucStrategyParameterFirmBB : UserControl
    {
        //Dependency property
        
        public static readonly DependencyProperty ucStrategyParameterFirmBBSymbolProperty = DependencyProperty.Register(
            "SelectedSymbol", 
            typeof(string), typeof(ucStrategyParameterFirmBB),
            new UIPropertyMetadata("", new PropertyChangedCallback(symbolChangedCallBack))
            );
        public static readonly DependencyProperty ucStrategyParameterFirmBBLayerProperty = DependencyProperty.Register(
            "SelectedLayer",
            typeof(string), typeof(ucStrategyParameterFirmBB),
            new UIPropertyMetadata("", new PropertyChangedCallback(layerChangedCallBack))
            );
        public static readonly DependencyProperty ucStrategyParameterFirmBBSelectedStrategyProperty = DependencyProperty.Register(
            "SelectedStrategy",
            typeof(string), typeof(ucStrategyParameterFirmBB),
            new UIPropertyMetadata("", new PropertyChangedCallback(strategyChangedCallBack))
            );
        
        public ucStrategyParameterFirmBB()
        {
            InitializeComponent();
            this.DataContext = new VisualHFT.ViewModel.vmStrategyParameterFirmBB(Helpers.HelperCommon.GLOBAL_DIALOGS);
            ((VisualHFT.ViewModel.vmStrategyParameterFirmBB)this.DataContext).IsActive = Visibility.Hidden;
        }

        
        public string SelectedSymbol
        {
            get { return (string)GetValue(ucStrategyParameterFirmBBSymbolProperty); }
            set { SetValue(ucStrategyParameterFirmBBSymbolProperty, value); ((VisualHFT.ViewModel.vmStrategyParameterFirmBB)this.DataContext).SelectedSymbol = value; }
        }
        public string SelectedLayer
        {
            get { return (string)GetValue(ucStrategyParameterFirmBBLayerProperty); }
            set { SetValue(ucStrategyParameterFirmBBLayerProperty, value); ((VisualHFT.ViewModel.vmStrategyParameterFirmBB)this.DataContext).SelectedLayer = value; }
        }
        public string SelectedStrategy
        {
            get { return (string)GetValue(ucStrategyParameterFirmBBSelectedStrategyProperty); }
            set { SetValue(ucStrategyParameterFirmBBSelectedStrategyProperty, value); ((VisualHFT.ViewModel.vmStrategyParameterFirmBB)this.DataContext).SelectedStrategy = value; }
        }
        static void symbolChangedCallBack(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            ucStrategyParameterFirmBB ucSelf = (ucStrategyParameterFirmBB)property;
            ucSelf.SelectedSymbol = (string)args.NewValue;
        }
        static void layerChangedCallBack(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            ucStrategyParameterFirmBB ucSelf = (ucStrategyParameterFirmBB)property;
            ucSelf.SelectedLayer = (string)args.NewValue;
        }
        static void strategyChangedCallBack(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            ucStrategyParameterFirmBB ucSelf = (ucStrategyParameterFirmBB)property;
            ucSelf.SelectedStrategy = (string)args.NewValue;
        }
    }
}
