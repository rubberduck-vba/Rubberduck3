using System;
using System.Globalization;
using System.Windows.Media;
using Rubberduck.UI.Shell.StatusBar;
using System.Collections.Generic;
using Rubberduck.Resources.Icons;
using System.Windows.Data;
using System.Linq;

namespace Rubberduck.UI.Converters
{
    public class ServerConnectionStateToIconConverter : ImageSourceConverter, IMultiValueConverter
    {
        private static readonly ImageSource DisconnectedIcon = ToImageSource(RubberduckIcons.circle_exclamation_solid);
        private static readonly ImageSource ConnectingIcon = ToImageSource(RubberduckIcons.hourglass_half_solid);
        private static readonly ImageSource ConnectedIcon = ToImageSource(RubberduckIcons.circle_check_solid);

        private static readonly Dictionary<ServerConnectionState, ImageSource> Mapping = new()
        {
            [ServerConnectionState.Disconnected] = DisconnectedIcon,
            [ServerConnectionState.Connecting] = ConnectingIcon,
            [ServerConnectionState.Connected] = ConnectedIcon,
        };

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (ServerConnectionState)value;
            return Mapping.TryGetValue(state, out var result) ? result : Mapping[ServerConnectionState.Disconnected];
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(values[0], targetType, parameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}