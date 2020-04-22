var Departments = [];
var element = $('#DepartmentOption');
var option = 0;
var optionName = "";
$(document).ready(function () {
    //Load Select Box Department
    //debugger;
    $.ajax({
        url: "/Departments/LoadDepartment",
        type: "GET",
        success: function (data) {
            //debugger;
            Departments = data.data;
            var $option = $(element);
            $option.empty();
            $option.append($('<option/>').val(option).text(optionName).hide());
            $.each(Departments, function (i, val) {
                //debugger;
                $option.append($('<option/>').val(val.id).text(val.name));
            });
        }
    });
    //Load Data From User Login
    //debugger;
    $.ajax({
        url: "/Users/LoadEmployee/",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
            //debugger;
            $('#Email').val(result.email);
            $('#FirstName').val(result.firstName);
            $('#LastName').val(result.lastName);
            $('#BirthDate').val(moment(result.birthDate).format('YYYY-MM-DD'));
            $('#PhoneNumber').val(result.phoneNumber);
            $('#Address').val(result.address);
            $('#DepartmentOption').val(result.department_Id);
            option = result.department_Id;
            optionName = result.departmentName;
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
});
