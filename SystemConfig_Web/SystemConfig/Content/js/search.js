var isFuncKey = false , inGrid = false;

function initSearchControl(opt) {
    var txtSearch = $("#" + opt.id);
    var valCtl = " <input id='" + opt.id + "_val" + "' name='" + opt.name + "' type='hidden'></input>";
    txtSearch.parent().append(valCtl);
    var divDataGrid = "<div id='" + opt.id + "_divDataGrid' style='position:fixed;z-index:999;background:white;border:1px solid darkgrey'> <table id='" + opt.id + "_dataGrid'></table></div>";
    txtSearch.parent().append(divDataGrid);
    if (opt.width == null)
        opt.width = 400;
    if (opt.height == null)
        opt.height = 300;
    init_Grid(opt);
    init_Control(opt);
}

function init_Control(opt) {
    var txtBox = $('#' + opt.id);
    var tBox = txtBox[0];
    var txtVal = $('#' + opt.id + '_val');
    var divDataGrid = $('#' + opt.id + '_divDataGrid');
    var dataGrid = $('#' + opt.id + '_dataGrid');
    var scroll = $('#unitSearch_divDataGrid .ui-jqgrid-bdiv');
    txtVal.val(opt.value == null ? null : opt.value);
    txtBox.val(opt.displayText);
    txtBox.attr('displayText', opt.displayText);
    tBox.onkeydown = function(e) {
        isFuncKey = true;
        if (e.keyCode == 13) {
            onSearchClose(opt, true);
        } else if (e.keyCode == 27) {
            onSearchClose(opt, false);
        } else if (e.keyCode == 38) {
            var ids = dataGrid.jqGrid('getDataIDs');
            if (ids.length > 0) {
                var sid = dataGrid.getGridParam('selrow');
                var index = ids.indexOf(sid);
                dataGrid.setSelection(ids[index - 1]);
                scroll.scrollTop(scroll.scrollTop() - 32);
                event.preventDefault();
            }
        } else if (e.keyCode == 40) {
            var ids = dataGrid.jqGrid('getDataIDs');
            if (ids.length > 0) {
                var sid = dataGrid.getGridParam('selrow');
                var index = ids.indexOf(sid);
                dataGrid.setSelection(ids[index + 1]);
                scroll.scrollTop(scroll.scrollTop() + 32);
                event.preventDefault();
            }
        } else if (e.keyCode == 46) {
            txtBox.val("");
            txtBox.attr("displayText", "");
            txtVal.val("");
        } else {
            isFuncKey = false;
            if (divDataGrid.css('display') == 'none')
                divDataGrid.css('display', 'block');
        }
    };
    tBox.onkeyup = function(e) {
        if (isFuncKey)
            return;
        var newData = new Array();
        var ds = opt.dataSource;
        var txt = $.trim(txtBox.val());
        if (opt.filter == null) {
            ds.forEach(function(obj) {
                for (var name in obj) {
                    if ((obj[name].toString()).indexOf(txt) >= 0) {
                        newData.push(obj);
                        break;
                    }
                }
            });
        } else {
            newData = opt.filter(ds, txt);
        }

        dataGrid.clearGridData();
        dataGrid.setGridParam({
            datatype: 'local',
            data: newData,
        }).trigger("reloadGrid");
    };
    tBox.onblur = function(e) {
        if (!inGrid) {
            onSearchClose(opt, false);
        }
    };
    var offset = txtBox.offset();
    offset.top += txtBox.height() + 12;
    divDataGrid.offset(offset);
    divDataGrid.width(opt.width);
    divDataGrid.height(opt.height);
    divDataGrid.css('display', 'none');
    divDataGrid[0].onmouseenter = function(e) {
        inGrid = true;
    };
    divDataGrid[0].onmouseleave = function(e) {
        tBox.focus();
        inGrid = false;
    };
}
function init_Grid(opt) {
    var dataGrid = $('#' + opt.id + '_dataGrid');
    dataGrid.jqGrid({
        datatype: 'local',
        data: opt.dataSource,
	rowNum: -1,
        colModel: opt.colModel,
        width: opt.width,
        height: opt.height - 45,
        ondblClickRow: function(id, irow, icol, e) {
            onSearchClose(opt, true);
        },
        loadComplete: function() {
            //选中第一行
            var ids = dataGrid.jqGrid('getDataIDs');
            if (ids.length > 0)
                dataGrid.setSelection(ids[0]);
        }
    });
}
function onSearchClose(opt, done) {
    var txtBox = $('#' + opt.id);
    var txtVal = $('#' + opt.id + '_val');
    var dataGrid = $('#' + opt.id + '_dataGrid');
    var divDataGrid = $('#' + opt.id + '_divDataGrid');
    if (divDataGrid.css('display') == 'block')
        divDataGrid.css('display', 'none');
    if (done) {
        var sid = dataGrid.getGridParam('selrow');
        var txt = dataGrid.getRowData(sid)[txtBox.attr('displayMember')];
        var val = dataGrid.getRowData(sid)[txtBox.attr('valueMember')];
        txtVal.val(val);
        txtBox.attr('displayText', txt);
        txtBox.val(txt);
    } else {
        txtBox.val(txtBox.attr('displayText'));
    }
}
