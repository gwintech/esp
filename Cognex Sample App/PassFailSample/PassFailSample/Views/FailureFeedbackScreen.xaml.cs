﻿using System;
using PassFailSample.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PassFailSample.Utilities.Navigation;

namespace PassFailSample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FailureFeedbackScreen : ContentPage, IViewFor
    {

        public ListView ListView;

        private BaseViewModel _ViewModel;
        public BaseViewModel ViewModel
        {
            get => this._ViewModel;
            set
            {
                this._ViewModel = value;
                BindingContext = this._ViewModel;
            }
        }

        public FailureFeedbackScreen(FailureFeedbackScreenViewModel vm)
        {
            InitializeComponent();
            this.ViewModel = vm;
            this.ListView = FailureFeedbackListView;
        }
    }
}