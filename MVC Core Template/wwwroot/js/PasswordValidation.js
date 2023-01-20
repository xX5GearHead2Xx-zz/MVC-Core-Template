var Regex = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{8,})");
const PasswordValidationToast = document.getElementById('PasswordValidationToast')
var ValidPassword = false;
var PasswordsMatched = false
function ValidatePassword(element) {
    ValidPassword = Regex.test(element.value);
    PasswordsMatch();
}

function PasswordsMatch() {
    var Password = document.getElementById("Password").value;
    var ConfirmPassword = document.getElementById("ConfirmPassword").value;

    PasswordsMatched = (Password == ConfirmPassword);
}

function SubmitForm(FormID) {
    if (ValidPassword && PasswordsMatched) {
        document.getElementById(FormID).submit();
    } else {
        const Toast = new bootstrap.Toast(PasswordValidationToast);
        if (!ValidPassword) {
            document.getElementById("PasswordErrorMessage").innerHTML = "Please enter a valid password";
            document.getElementById("PasswordPolicy").style.display = "block";
            Toast.show();
        } else if (!PasswordsMatched) {
            document.getElementById("PasswordErrorMessage").innerHTML = "Passwords do not match";
            document.getElementById("PasswordPolicy").style.display = "none";
            Toast.show();
        }
    }
}