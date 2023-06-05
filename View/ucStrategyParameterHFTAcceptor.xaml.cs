﻿using System.Windows;
using System.Windows.Controls;

namespace VisualHFT.View
{
    /// <summary>
    /// Interaction logic for ucStrategyParameterFirmMM.xaml
    /// </summary>
    public partial class ucStrategyParameterHFTAcceptor : UserControl
    {
        //Dependency property
        public static readonly DependencyProperty ucStrategyParameterHFTAcceptorSymbolProperty = DependencyProperty.Register(
            "SelectedSymbol", 
            typeof(string), typeof(ucStrategyParameterHFTAcceptor),
            new UIPropertyMetadata("", new PropertyChangedCallback(symbolChangedCallBack))
            );
        public static readonly DependencyProperty ucStrategyParameterHFTAcceptorLayerProperty = DependencyProperty.Register(
            "SelectedLayer",
            typeof(string), typeof(ucStrategyParameterHFTAcceptor),
            new UIPropertyMetadata("", new PropertyChangedCallback(layerChangedCallBack))
            );
        public static readonly DependencyProperty ucStrategyParameterHFTAcceptorSelectedStrategyProperty = DependencyProperty.Register(
            "SelectedStrategy",
            typeof(string), typeof(ucStrategyParameterHFTAcceptor),
            new UIPropertyMetadata("", new PropertyChangedCallback(strategyChangedCallBack))
            );

        public ucStrategyParameterHFTAcceptor()
        {
            InitializeComponent();
            this.DataContext = new VisualHFT.ViewModel.vmStrategyParameterHFTAcceptor(Helpers.HelperCommon.GLOBAL_DIALOGS);
            ((VisualHFT.ViewModel.vmStrategyParameterHFTAcceptor)this.DataContext).IsActive = Visibility.Hidden;
        }

        public string SelectedSymbol
        {
            get { return (string)GetValue(ucStrategyParameterHFTAcceptorSymbolProperty); }
            set { SetValue(ucStrategyParameterHFTAcceptorSymbolProperty, value); ((VisualHFT.ViewModel.vmStrategyParameterHFTAcceptor)this.DataContext).SelectedSymbol = value; }
        }
        public string SelectedLayer
        {
            get { return (string)GetValue(ucStrategyParameterHFTAcceptorLayerProperty); }
            set { SetValue(ucStrategyParameterHFTAcceptorLayerProperty, value); ((VisualHFT.ViewModel.vmStrategyParameterHFTAcceptor)this.DataContext).SelectedLayer = value; }
        }
        public string SelectedStrategy
        {
            get { return (string)GetValue(ucStrategyParameterHFTAcceptorSelectedStrategyProperty); }
            set { SetValue(ucStrategyParameterHFTAcceptorSelectedStrategyProperty, value); ((VisualHFT.ViewModel.vmStrategyParameterHFTAcceptor)this.DataContext).SelectedStrategy = value; }
        }
        static void symbolChangedCallBack(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
			ucStrategyParameterHFTAcceptor ucSelf = (ucStrategyParameterHFTAcceptor)property;
            ucSelf.SelectedSymbol = (string)args.NewValue;
        }
        static void layerChangedCallBack(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
			ucStrategyParameterHFTAcceptor ucSelf = (ucStrategyParameterHFTAcceptor)property;
            ucSelf.SelectedLayer = (string)args.NewValue;
        }
        static void strategyChangedCallBack(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            ucStrategyParameterHFTAcceptor ucSelf = (ucStrategyParameterHFTAcceptor)property;
            ucSelf.SelectedStrategy = (string)args.NewValue;
        }

    }
}
