function AddContact() {
    var CurrentCount = parseInt($("#ContactCount").val());
    if (CurrentCount < 5) {
        CurrentCount++;
        $("#ContactCount").val(CurrentCount);
        $("#Contacts").append(
            "<div class='row mt-1 mb-1' id='Contact_" + CurrentCount + "'>" +
            "<input type='hidden' name='ContactID_" + CurrentCount + "' id='ContactID_" + CurrentCount + "'/>" +
            "<div class='col-6 mt-1'>" +
            "<div class='form-group'>" +
            "<input class='form-control' type='text' name='ContactName_" + CurrentCount + "' id='ContactName_" + CurrentCount + "' placeholder='Name' required>" +
            "</div>" +
            "</div>" +
            "<div class='col-6 mt-1'>" +
            "<div class='form-group'>" +
            "<input class='form-control' type='number' name='ContactNumber_" + CurrentCount + "' id='ContactNumber_" + CurrentCount + "' placeholder='Number' required>" +
            "</div>" +
            "</div>" +
            "</div>");
    } else {
        alert("You may only add 5 contacts");
    }

}

function RemoveContact() {
    var CurrentCount = parseInt($("#ContactCount").val());
    if (CurrentCount > 0) {
        var RowID = "#Contact_" + CurrentCount;
        var ContactID = $("#ContactID_" + CurrentCount).val();
        if (ContactID !== "") {
            DeleteContact(ContactID);
        }
        $(RowID).remove();
        CurrentCount--;
        $("#ContactCount").val(CurrentCount);
    }
}

function ClearAddressForm() {
    $("#EditAddressID").val('');
    $("#PropertyType").val('');
    $("#StreetAddress").val('');
    $("#Suburb").val('');
    $("#Code").val('');
    $("#City").val('');
    $("#Province").val('');
    $("#DeliveryInstructions").val('');
    $("#ContactCount").val(0);
    $("#Contacts").empty();
}

function DeleteContact(ContactID) {
    try {
        $.ajax({
            cache: false,
            url: "/AccountManagement/DeleteContact",
            type: 'GET',
            datatype: 'json',
            data: {
                'ContactID': ContactID
            },
            success: function (Response) {
                if (Response == "Success") {
                    DisplayNotification("Contact Deleted", "The contact has been deleted successfully", "Success");
                }
            }
        });
    } catch (err) {
        console.log(err);
        DisplayNotification("Error", "Something went wrong while processing the request", "Error");
    }
}

var AddressModal = new bootstrap.Modal(document.getElementById("AddressModal"), {});


function ConfirmDeleteAddress(element) {
    dialog('Are you sure you want to delete the address?',
        function () {
            DeleteAddress(element);
        }
    );
}

function DeleteAddress(element) {
    try {
        var AddressID = $(element).attr('AddressID');
        var AddressNumber = $(element).attr('AddressNumber');
        $.ajax({
            cache: false,
            url: "/AccountManagement/DeleteAddress",
            type: 'GET',
            datatype: 'json',
            data: {
                'AddressID': AddressID
            },
            success: function (Response) {
                if (Response == "Success") {
                    var AddressColumnID = "#Address_" + AddressNumber;
                    $(AddressColumnID).remove();
                    DisplayNotification("Address Deleted", "The address has been deleted successfully", "Success");
                }
            }
        });
    } catch (err) {
        console.log(err);
        DisplayNotification("Error", "Something went wrong while processing the request", "Error");
    }
}

function EditAddress(element) {
    try {
        ClearAddressForm();
        var AddressID = $(element).attr('AddressID');
        $.ajax({
            cache: false,
            url: "/AccountManagement/LoadAddress",
            type: 'GET',
            datatype: 'json',
            data: {
                'AddressID': AddressID
            },
            success: function (Response) {
                $("#EditAddressID").val(Response.key);
                $("#PropertyType").val(Response.addressType);
                $("#StreetAddress").val(Response.streetAddress);
                $("#Suburb").val(Response.suburb);
                $("#Code").val(Response.code);
                $("#City").val(Response.city);
                $("#Province").val(Response.province);
                $("#DeliveryInstructions").val(Response.deliveryInstructions);
            }
        });

        $.ajax({
            cache: false,
            url: "/AccountManagement/LoadContacts",
            type: 'GET',
            datatype: 'json',
            data: {
                'AddressID': AddressID
            },
            success: function (Response) {
                var Counter = 1;
                Response.forEach(function (Contact) {
                    console.log(Contact);
                    AddContact();
                    $("#ContactID_" + Counter).val(Contact.key);
                    $("#ContactName_" + Counter).val(Contact.name);
                    $("#ContactNumber_" + Counter).val(Contact.value);
                    Counter++;
                })
            }
        });
        AddressModal.show();
    } catch (err) {
        console.log(err);
        DisplayNotification("Error", "Something went wrong while processing the request", "Error");
    }
}

