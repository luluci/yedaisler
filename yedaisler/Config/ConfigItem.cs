using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yedaisler.Config
{
    internal interface IApplyOrCancel
    {
        bool IsAttachApply { get; set; }
        void ApplyOrCancel(bool apply);
    }

    internal class ConfigItem<T> : IApplyOrCancel
    {
        public delegate void WriteBackHandler(T newValue);

        public bool IsAttachApply {  get; set; }
        public bool IsChanged { get; set; }
        public ReactiveProperty<T> Model { get; set; }
        public ReactiveProperty<T> View { get; set; }
        public WriteBackHandler WriteBack {  get; set; }

        public ConfigItem(T value)
        {
            IsAttachApply = false;
            WriteBack = null;
            IsChanged = false;
            Model = new ReactiveProperty<T>(value);
            View = new ReactiveProperty<T>(value);
            View.Subscribe(x =>
            {
                if (Model.Value.Equals(x))
                {
                    IsChanged = false;
                }
                else
                {
                    IsChanged = true;
                }
            });
        }

        public void ApplyOrCancel(bool apply)
        {
            if (IsChanged)
            {
                if (apply)
                {
                    Model.Value = View.Value;
                }
                else
                {
                    View.Value = Model.Value;
                }
            }
        }
    }
}
