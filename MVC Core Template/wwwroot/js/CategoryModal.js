var CategoryModal = new bootstrap.Modal(document.getElementById("CategoryModal"), {});

function ClearCategoryForm() {
    $("#CategoryDepartmentID").val('');
    $("#EditCategoryID").val('');
    $("#CategoryName").val('');
}

function SetCategoryDepartmentID(element) {
    var DepartmentID = $(element).attr('DepartmentID');
    $("#CategoryDepartmentID").val(DepartmentID);
}

function ConfirmDeleteCategory(element) {
    dialog('Are you sure you want to delete the category?',
        function () {
            DeleteCategory(element);
        }
    );
}

function HideUnhideCategory(element) {
    try {
        var CategoryID = $(element).attr('CategoryID');
        var Hidden = $(element).attr("Hidden");
        var CategoryNumber = $(element).attr('CategoryNumber');
        $.ajax({
            cache: false,
            url: "/DepartmentManagement/HideUnhideCategory",
            type: 'GET',
            datatype: 'json',
            data: {
                'CategoryID': CategoryID,
                'Hidden': Hidden
            },
            success: function (Response) {
                if (Response == "Success") {
                    var CategoryColumnID = "#Category_" + CategoryNumber;
                    $(CategoryColumnID).remove();
                    DisplayNotification("Category Deleted", "The category has been deleted successfully", "Success");
                }
            }
        });
    } catch (err) {
        console.log(err);
        DisplayNotification("Error", "Something went wrong while processing the request", "Error");
    }

}

function EditCategory(element) {
    try {
        var CategoryID = $(element).attr('CategoryID');
        var DepartmentID = $(element).attr('DepartmentID');
        var DepartmentNumber = $(element).attr('DepartmentNumber');
        var CategoryNumber = $(element).attr('CategoryNumber');

        $.ajax({
            cache: false,
            url: "/DepartmentManagement/LoadCategory",
            type: 'GET',
            datatype: 'json',
            data: {
                'CategoryID': CategoryID
            },
            success: function (Response) {
                $("#CategoryName").val(Response.description);
                $("#SaveCategoryButton").attr("DepartmentID", DepartmentID);
                $("#SaveCategoryButton").attr("DepartmentNumber", DepartmentNumber);
                $("#SaveCategoryButton").attr("EditCategoryID", Response.key);
                $("#SaveCategoryButton").attr("CategoryNumber", CategoryNumber);
            }
        });

        CategoryModal.show();
    } catch (err) {
        console.log(err);
        DisplayNotification("Error", "Something went wrong while processing the request", "Error");
    }
}

function AddCategory(element) {
    try {
        ClearCategoryForm();
        var DepartmentID = $(element).attr('DepartmentID');
        var DepartmentNumber = $(element).attr('DepartmentNumber');
        $("#SaveCategoryButton").attr("DepartmentID", DepartmentID);
        $("#SaveCategoryButton").attr("DepartmentNumber", DepartmentNumber);
        CategoryModal.show();
    } catch (err) {
        console.log(err);
        DisplayNotification("Error", "Something went wrong while processing the request", "Error");
    }
};

function SaveCategory(element) {
    try {
        var DepartmentID = $(element).attr('DepartmentID');
        var DepartmentNumber = $(element).attr('DepartmentNumber');
        var CategoryName = $("#CategoryName").val();
        var EditCategoryID = $(element).attr('EditCategoryID');

        if (EditCategoryID !== "") {
            var CategoryNumber = $(element).attr('CategoryNumber');
            RemoveCategoryRow(CategoryNumber);
        }

        $.ajax({
            cache: false,
            url: "/DepartmentManagement/SaveCategory",
            type: 'GET',
            datatype: 'json',
            data: {
                'DepartmentCategoryID': DepartmentID,
                'CategoryName': CategoryName,
                'EditCategoryID': EditCategoryID
            },
            success: function (Response) {
                CategoryModal.hide();
                AddCategoryRow(Response, DepartmentNumber);
            }
        });
    } catch (error) {
        console.log(err);
        DisplayNotification("Error", "Something went wrong while processing the request", "Error");
    }
};

function IncrementCategoryCount() {
    var CategoryCount = $("#CategoryCount").val();
    CategoryCount += 1;
    $("#CategoryCount").val(CategoryCount);
}

function RemoveCategoryRow(CategoryCount) {
    $("#Category_" + CategoryCount).remove();
}

function AddCategoryRow(Category, DepartmentNumber) {
    var Table = document.getElementById("Categories_" + DepartmentNumber);
    var Row = Table.insertRow(1);
    var CategoryName = Row.insertCell(0);
    var Hidden = Row.insertCell(1);
    var Buttons = Row.insertCell(2);

    IncrementCategoryCount();
    var CategoryCount = $("#CategoryCount").val();

    CategoryName.innerHTML = Category.description;

    if (Category.Hidden == "true") {
        Hidden.innerHTML = "Yes";
    } else {
        Hidden.innerHTML = "No";
    }

    Buttons.innerHTML = "<span role='button' title='Edit' class='text-primary p-1' CategoryID='" + Category.key + "' CategoryNumber='" + CategoryCount + "' onclick='EditCategory(this)'><i class='fa-solid fa-pen-to-square'></i></span>" +
        "<span role='button' title='Hide/Unhide' class='text-dark p-1' CategoryID='" + Category.key + "' CategoryHidden='" + Category.hidden + "' CategoryNumber='" + CategoryCount + "' onclick='HideUnhideCategory(this)'><i class='fa-solid fa-eye-slash'></i></span>"

    Row.setAttribute("id", "Category_" + CategoryCount);
    Row.classList.add("animate__animated");
    Row.classList.add("animate__flash");
}

function HideUnhideCategory(element) {
    var CategoryID = $(element).attr('CategoryID');
    var CategoryNumber = $(element).attr('CategoryNumber');
    var CategoryHidden = $(element).attr('CategoryHidden');

    var Hide = "";
    switch (CategoryHidden) {
        case "True":
            Hide = "False";
            break;
        case "False":
            Hide = "True";
            break
    }

    $.ajax({
        cache: false,
        url: "/DepartmentManagement/HideUnhideCategory",
        type: 'GET',
        datatype: 'json',
        data: {
            'CategoryID': CategoryID,
            'Hide': Hide,
        },
        success: function (Response) {
            if (Response == "Success") {
                var Row = $("Category_" + CategoryNumber);

                if (Hide == "true") {
                    Row.cells[1].innerHTML("Yes");
                } else {
                    Row.cells[1].innerHTML("No");
                }
                var Buttons = Row.cells(2);
                Buttons.empty();
                Buttons.innerHTML = "<span role='button' title='Edit' class='text-primary p-1' CategoryID='" + CategoryID + "' CategoryNumber='" + CategoryNumber + "' onclick='EditCategory(this)'><i class='fa-solid fa-pen-to-square'></i></span>" +
                    "<span role='button' title='Hide/Unhide' class='text-dark p-1' CategoryID='" + CategoryID + "' CategoryHidden='" + Hide + "' CategoryNumber='" + CategoryNumber + "' onclick='HideUnhideCategory(this)'><i class='fa-solid fa-eye-slash'></i></span>"

            }
        }
    });
}