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

    internal class ConfigItemApplier
    {
        private List<IApplyOrCancel> applier;

        public ConfigItemApplier()
        {
            applier = new List<IApplyOrCancel>();
        }


        public void Add(IApplyOrCancel apply)
        {
            if (!apply.IsAttachApply)
            {
                apply.IsAttachApply = true;
                applier.Add(apply);
            }
        }

        public void Confirm(bool apply)
        {
            foreach (var item in applier)
            {
                item.IsAttachApply = false;
                item.ApplyOrCancel(apply);
            }
            //
            applier.Clear();
        }

        public void Apply()
        {
            Confirm(true);
        }
        public void Cancel()
        {
            Confirm(false);
        }

    }

    internal class ConfigItem<T> : IApplyOrCancel
    {
        public delegate void WriteBackHandler(T newValue);
        public delegate bool ValidateHandler(T newValue);

        public bool IsAttachApply {  get; set; }
        public bool IsChanged { get; set; }
        public ReactiveProperty<T> Model { get; set; }
        public ReactiveProperty<T> View { get; set; }
        public WriteBackHandler WriteBack {  get; set; }
        public ValidateHandler Validator { get; set; }

        public ConfigItem(T value, ConfigItemApplier applier)
        {
            IsAttachApply = false;
            WriteBack = null;
            Validator = null;
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
                    applier.Add(this);

                    if (!(Validator is null))
                    {

                    }
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
                    if (!(WriteBack is null))
                    {
                        WriteBack(View.Value);
                    }
                }
                else
                {
                    View.Value = Model.Value;
                }
            }
        }
    }
}
