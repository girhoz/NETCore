var table = null;
var dateNow = new Date();
var Departments = [];
$(document).ready(function () {
    table = $('#Employees').DataTable({ //Nama table pada index
        "ajax": {
            url: "/Employees/LoadEmployee", //Nama controller/fungsi pada index controller
            type: "GET",
            dataType: "json",
            dataSrc: ""
        },
        "columnDefs": [
            { "orderable": false, "targets": 8 },
            { "searchable": false, "targets": 8 }
        ],
        "columns": [
            { "data": "name", "name": "Name" },
            { "data": "departmentName", "name": "Department Name" },
            { "data": "email", "name": "Email" },
            {
                "data": "birthDate", "render": function (data) {
                    return moment(data).format('DD/MM/YYYY');
                }
            },
            { "data": "phoneNumber", "name": "Phone Number" },
            { "data": "address", "name": "Address" },
            {
                "data": "createDate", "render": function (data) {
                    return moment(data).format('DD/MM/YYYY');
                }
            },
            {
                "data": "updateDate", "render": function (data) {
                    var dateupdate = "Not Updated Yet";
                    var nulldate = null;
                    if (data === nulldate) {
                        return dateupdate;
                    } else {
                        return moment(data).format('DD/MM/YYYY');
                    }
                }
            },
            {
                data: null, render: function (data, type, row) {
                    return " <td><button type='button' class='btn btn-warning' Id='Update' onclick=GetById('" + row.id + "');>Edit</button> <button type='button' class='btn btn-danger' Id='Delete' onclick=Delete('" + row.id + "');>Delete</button ></td >";
                }
            }
        ]
    });
});

//Tampung dan tampilkan department kedalam dropdownlist
function LoadDepartment(element) {
    if (Departments.length === 0) {
        $.ajax({
            type: "Get",
            url: "/Departments/LoadDepartment",
            success: function (data) {
                Departments = data.data;
                renderDepartment(element);
            }
        });
    }
    else {
        renderDepartment(element);
    }
}

function renderDepartment(element) {
    var $option = $(element);
    $option.empty();
    $option.append($('<option/>').val('0').text('Select Department').hide());
    $.each(Departments, function (i, val) {
        $option.append($('<option/>').val(val.id).text(val.name));
    });
}
LoadDepartment($('#DepartmentOption'));

function Save() {
    //debugger;
    var Employee = new Object();
    Employee.FirstName = $('#FirstName').val();
    Employee.LastName = $('#LastName').val();
    Employee.Email = $('#Email').val();
    Employee.BirthDate = $('#BirthDate').val();
    Employee.PhoneNumber = $('#PhoneNumber').val();
    Employee.Address = $('#Address').val();
    Employee.Department_Id = $('#DepartmentOption').val();
    $.ajax({
        type: 'POST',
        url: '/Employees/InsertOrUpdate/',
        data: Employee
    }).then((result) => {
        //debugger;
        if (result.statusCode === 200) {
            Swal.fire({
                position: 'center',
                type: 'success',
                title: 'Employee Added Succesfully'
            }).then((result) => {
                if (result.value) {
                    table.ajax.reload();
                }
            });
        }
        else {
            Swal.fire('Error', 'Failed to Add Employee', 'error');
            ShowModal();
        }
    });
}

function GetById(Id) {
    $.ajax({
        url: "/Employees/GetById/" + Id,
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: false,
        success: function (result) {
            //debugger;
            $('#Id').val(result[0].id);
            $('#FirstName').val(result[0].firstName);
            $('#LastName').val(result[0].lastName);
            $('#Email').val(result[0].email);
            $('#BirthDate').val(result[0].birthDate);
            $('#PhoneNumber').val(result[0].phoneNumber);
            $('#Address').val(result[0].address);
            $('#DepartmentOption').val(result[0].department_Id);
            $("#createModal").modal('show');
            $("#Save").hide();
            $('#Edit').show();
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

function Edit() {
    var Employee = new Object();
    Employee.Id = $('#Id').val();
    Employee.FirstName = $('#FirstName').val();
    Employee.LastName = $('#LastName').val();
    Employee.Email = $('#Email').val();
    Employee.BirthDate = $('#BirthDate').val();
    Employee.PhoneNumber = $('#PhoneNumber').val();
    Employee.Address = $('#Address').val();
    Employee.Department_Id = $('#DepartmentOption').val();
    $.ajax({
        type: 'POST',
        url: '/Employees/InsertOrUpdate/',
        data: Employee
    }).then((result) => {
        //debugger;
        if (result.statusCode === 200) {
            Swal.fire({
                position: 'center',
                type: 'success',
                title: 'Employee Edit Succesfully'
            }).then((result) => {
                if (result.value) {
                    table.ajax.reload();
                }
            });
        }
        else {
            Swal.fire('Error', 'Failed to Edit Employee', 'error');
            ShowModal();
        }
    });
}

function Delete(Id) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        //debugger;
        if (result.value) {
            $.ajax({
                url: "/Employees/Delete/" + Id,
                data: { Id: Id }
            }).then((result) => {
                if (result.statusCode === 200) {
                    Swal.fire({
                        position: 'center',
                        type: 'success',
                        title: 'Employees Deleted Succesfully'
                    }).then((result) => {
                        if (result.value) {
                            table.ajax.reload();
                        }
                    });
                }
                else {
                    Swal.fire('Error', 'Failed to Delete Employees', 'error');
                    ShowModal();
                }
            });
        }
    });
}


function ShowModal() {
    $("#createModal").modal('show');
    $('#Id').val('');
    $('#FirstName').val('');
    $('#LastName').val('');
    $('#Email').val('');
    $('#BirthDate').val('');
    $('#PhoneNumber').val('');
    $('#Address').val('');
    $("#Save").show();
    $("#Edit").hide();
}