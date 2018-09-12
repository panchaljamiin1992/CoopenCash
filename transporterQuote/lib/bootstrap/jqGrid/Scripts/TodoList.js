$(function () {
    $("#grid").jqGrid({
        url: "/TodoList/GetTodoLists",
        datatype: 'json',
        mtype: 'Get',
        colNames: ['Id', 'Task Name', 'Task Description', 'Target Date', 'Severity', 'Task Status'],
        colModel: [
            { key: true, hidden: true, name: 'Id', index: 'Id', editable: true },
            { key: false, name: 'TaskName', index: 'TaskName', editable: true },
            { key: false, name: 'TaskDescription', index: 'TaskDescription', editable: true },
            { key: false, name: 'TargetDate', index: 'TargetDate', editable: true, formatter: 'date', formatoptions: { newformat: 'd/m/Y' } },
            { key: false, name: 'Severity', index: 'Severity', editable: true, edittype: 'select', editoptions: { value: { 'L': 'Low', 'M': 'Medium', 'H': 'High' } } },
            { key: false, name: 'TaskStatus', index: 'TaskStatus', editable: true, edittype: 'select', editoptions: { value: { 'A': 'Active', 'I': 'InActive' } } }],
        pager: jQuery('#pager'),
        rowNum: 10,
        rowList: [10, 20, 30, 40],
        height: '100%',
        viewrecords: true,
        caption: '',
        emptyrecords: 'No records to display',
        jsonReader: {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false,
            Id: "0"
        },
        autowidth: true,
        multiselect: false
    }).navGrid('#pager', { edit: false, add: false, del: false, search: true, refresh: true });
});