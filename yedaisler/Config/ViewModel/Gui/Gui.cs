using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yedaisler.Config.ViewModel.Gui
{
    internal class Gui
    {
        public Color Color { get; set; }

        public ConfigItem<StartupPosition> StartupPosition { get; set; }

        public Gui(ConfigItemApplier applier)
        {
            Color = new Color(applier);

            //
            StartupPosition = new ConfigItem<StartupPosition>(yedaisler.Config.StartupPosition.None, applier);
        }
    }

}
