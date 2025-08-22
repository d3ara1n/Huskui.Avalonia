using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Huskui.Avalonia.Controls;

namespace Huskui.Gallery.Dialogs
{
    public partial class EmailInputDialog : Dialog
    {
        private static readonly Regex EmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
                                                       RegexOptions.Compiled);

        public EmailInputDialog()
        {
            InitializeComponent();

            // 初始化时验证一次
            UpdateValidation();
        }

        protected override bool ValidateResult(object? result)
        {
            var email = result as string;
            var isValid = !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);

            // 更新UI验证状态
            UpdateValidationUI(isValid, email);

            return isValid;
        }

        private void UpdateValidation() =>
            // 触发验证更新
            ValidateResult(Result);

        private void UpdateValidationUI(bool isValid, string? email)
        {
            var validationBorder = this.FindControl<InfoBar>("ValidationBorder");
            var validationText = this.FindControl<TextBlock>("ValidationText");

            if (validationBorder == null || validationText == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                validationBorder.IsVisible = false;
            }
            else if (!isValid)
            {
                validationBorder.IsVisible = true;
                validationText.Text = "Please enter a valid email address (e.g., user@example.com)";
            }
            else
            {
                validationBorder.IsVisible = false;
            }
        }

        private void OnEmailTextBoxKeyDown(object? sender, KeyEventArgs e)
        {
            // 延迟验证，让TextBox先更新绑定
            Dispatcher.UIThread.Post(UpdateValidation);

            // 支持Enter键确认
            if (e.Key == Key.Enter && ValidateResult(Result))
            {
                Dismiss();
            }
        }
    }
}
