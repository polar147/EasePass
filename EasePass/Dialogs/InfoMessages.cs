﻿using EasePass.Extensions;
using Microsoft.UI.Xaml.Controls;


namespace EasePass.Dialogs
{
    internal class InfoMessages
    {
        public static void EnteredWrongPassword(int attempts) => new InfoBar().Show("Wrong password", $"You entered the wrong password.\nPlease try again\n({attempts}/3)", InfoBarSeverity.Error);
        public static void TooManyPasswordAttempts() => new InfoBar().Show("Too many attempts", "You entered the password wrong three times.", InfoBarSeverity.Error);
        public static void ExportDBWrongPassword() => new InfoBar().Show("Wrong password", "Could not import database, because you've entered the wrong password.", InfoBarSeverity.Error);
        public static void ImportDBSuccess() => new InfoBar().Show("Imported successfully", "Imported data successfully", InfoBarSeverity.Success);
        public static void ExportDBSuccess() => new InfoBar().Show("Export successfully", "Exported data successfully", InfoBarSeverity.Success);
        public static void PasswordTooShort() => new InfoBar().Show("Password too short", "The password is too short", InfoBarSeverity.Error);
        public static void ChangePasswordWrong() => new InfoBar().Show("Wrong password", "The current password is incorrect", InfoBarSeverity.Error);
        public static void PasswordsDoNotMatch() => new InfoBar().Show("Passwords do not match", "", InfoBarSeverity.Error);
        public static void SuccessfullyChangedPassword() => new InfoBar().Show("Password successfully changed", "Your password was successfully changed", InfoBarSeverity.Success);
        public static void AutomaticallyLoggedOut() => new InfoBar().ShowWithoutTimer("We automatically logged you out due to inactivity", "", InfoBarSeverity.Informational);
    }
}
