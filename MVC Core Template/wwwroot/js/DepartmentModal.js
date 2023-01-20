var DepartmentModal = new bootstrap.Modal(document.getElementById("DepartmentModal"), {});

function ClearDepartmentForm() {
    $("#EditDepartmentID").val('');
    $("#DepartmentName").val('');
}

function ConfirmDeleteDepartment(element) {
    dialog('Are you sure you want to delete the department?',
        function () {
            DeleteDepartment(element);
        }
    );
}

function HideUnhideDepartment(element) {
    try {
        var DepartmentID = $(element).attr('DepartmentID');
        var Hidden = $(element).attr("Hidden");
        var DepartmentNumber = $(element).attr('DepartmentNumber');
        $.ajax({
            cache: false,
            url: "/DepartmentManagement/HideUnhideDepartment",
            type: 'GET',
            datatype: 'json',
            data: {
                'DepartmentID': DepartmentID,
                'Hidden': Hidden
            },
            success: function (Response) {
                if (Response == "Success") {
                    var DepartmentColumnID = "#Department_" + DepartmentNumber;
                    $(DepartmentColumnID).remove();
                    DisplayNotification("Department Deleted", "The department has been deleted successfully", "Success");
                }
            }
        });
    } catch (err) {
        console.log(err);
        DisplayNotification("Error", "Something went wrong while processing the request", "Error");
    }

}

function EditDepartment(element) {
    try {
        ClearDepartmentForm();
        var DepartmentID = $(element).attr('DepartmentID');
        $.ajax({
            cache: false,
            url: "/DepartmentManagement/LoadDepartment",
            type: 'GET',
            datatype: 'json',
            data: {
                'DepartmentID': DepartmentID
            },
            success: function (Response) {
                $("#EditDepartmentID").val(Response.key);
            }
        });

        DepartmentModal.show();
    } catch (err) {
        console.log(err);
        DisplayNotification("Error", "Something went wrong while processing the request", "Error");
    }
}

