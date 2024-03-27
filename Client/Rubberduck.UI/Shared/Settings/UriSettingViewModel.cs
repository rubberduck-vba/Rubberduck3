using AsyncAwaitBestPractices;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.UI.Command;
using Rubberduck.UI.Shared.Settings.Abstract;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.UI.Shared.Settings
{
    public class UriSettingViewModel : SettingViewModel<Uri>, IBrowseFolderModel
    {
        public UriSettingViewModel(UriRubberduckSetting setting) : base(setting)
        {
            CheckIfExists();
        }

        private void CheckIfExists()
        {
            IsBusy = true;
            CheckExistsAsync(CancellationToken.None).SafeFireAndForget();
        }

        public Uri RootUri
        {
            get => Value;
            set
            {
                Value = value;

                IsBusy = true;
                CheckExistsAsync(CancellationToken.None).SafeFireAndForget();
            }
        }
        public string Title { get; set; }
        public string Selection 
        {
            get => Value.IsFile ? Value.LocalPath : Value.OriginalString;
            set
            {
                if (Value.OriginalString != value)
                {
                    Value = new Uri(value);
                    OnPropertyChanged();
                    CheckIfExists();
                }
            }
        }

        private bool _exists;
        public bool Exists 
        {
            get => _exists;
            private set
            {
                if (_exists != value)
                {
                    _exists = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _busy;
        public bool IsBusy
        {
            get => _busy;
            private set
            {
                if (_busy != value)
                {
                    _busy = value;
                    OnPropertyChanged();
                }
            }
        }

        private async Task CheckExistsAsync(CancellationToken token)
        {
            var result = false;
            IsBusy = true;

            if (Value.IsFile || Value.IsUnc)
            {
                result = System.IO.Path.Exists(Value.AbsolutePath);
            }

            if (Value.Scheme.StartsWith("http"))
            {
                try
                {
                    var url = Value.OriginalString;
                    using var client = new HttpClient() { Timeout = TimeSpan.FromSeconds(5) };
                    using var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(url + "/public/test")
                    };
                    request.Headers.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("rubberduck.editor", "3.0"));
                    request.Method = HttpMethod.Get;
                    using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
                    result = response.IsSuccessStatusCode;
                }
                catch (Exception)
                {
                }
            }

            IsBusy = false;
            Exists = result;
        }
    }
}
