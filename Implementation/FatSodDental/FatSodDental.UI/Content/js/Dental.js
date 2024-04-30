//document.onload = alert('bonjour');

window.onload = function () {

    Ext.selection.CheckboxModel.override({
        selectAll: function (suppressEvent) {
            var me = this,
                selections = me.store.getAllRange(), // instead of the getRange call
                i = 0,
                len = selections.length,
                start = me.getSelection().length;

            me.suspendChanges();

            for (; i < len; i++) {
                me.doSelect(selections[i], true, suppressEvent);
            }

            me.resumeChanges();
            if (!suppressEvent) {
                me.maybeFireSelectionChange(me.getSelection().length !== start);
            }
        },

        deselectAll: Ext.Function.createSequence(Ext.selection.CheckboxModel.prototype.deselectAll, function () {
            this.view.panel.getSelectionMemory().clearMemory();
        }),

        updateHeaderState: function () {
            var me = this,
                store = me.store,
                storeCount = store.getTotalCount(),
                views = me.views,
                hdSelectStatus = false,
                selectedCount = 0,
                selected, len, i;

            if (!store.buffered && storeCount > 0) {
                selected = me.view.panel.getSelectionMemory().selectedIds;
                hdSelectStatus = true;
                for (s in selected) {
                    ++selectedCount;
                }

                hdSelectStatus = storeCount === selectedCount;
            }

            if (views && views.length) {
                me.toggleUiHeader(hdSelectStatus);
            }
        }
    });

    Ext.grid.plugin.SelectionMemory.override({
        memoryRestoreState: function (records) {
            if (this.store !== null && !this.store.buffered && !this.grid.view.bufferedRenderer) {
                var i = 0,
                    ind,
                    sel = [],
                    len,
                    all = true,
                    cm = this.headerCt;

                if (!records) {
                    records = this.store.getAllRange(); // instead of getRange
                }

                if (!Ext.isArray(records)) {
                    records = [records];
                }

                if (this.selModel.isLocked()) {
                    this.wasLocked = true;
                    this.selModel.setLocked(false);
                }

                if (this.selModel instanceof Ext.selection.RowModel) {
                    for (ind = 0, len = records.length; ind < len; ind++) {
                        var rec = records[ind],
                            id = rec.getId();

                        if ((id || id === 0) && !Ext.isEmpty(this.selectedIds[id])) {
                            sel.push(rec);
                        } else {
                            all = false;
                        }

                        ++i;
                    }

                    if (sel.length > 0) {
                        this.surpressDeselection = true;
                        this.selModel.select(sel, false, !this.grid.selectionMemoryEvents);
                        this.surpressDeselection = false;
                    }
                } else {
                    for (ind = 0, len = records.length; ind < len; ind++) {
                        var rec = records[ind],
                            id = rec.getId();

                        if ((id || id === 0) && !Ext.isEmpty(this.selectedIds[id])) {
                            if (this.selectedIds[id].dataIndex) {
                                var colIndex = cm.getHeaderIndex(cm.down('gridcolumn[dataIndex=' + this.selectedIds[id].dataIndex + ']'))
                                this.selModel.setCurrentPosition({
                                    row: i,
                                    column: colIndex
                                });
                            }
                            return false;
                        }

                        ++i;
                    }
                }

                if (this.selModel instanceof Ext.selection.CheckboxModel) {
                    if (all && (records.length > 0)) {
                        this.selModel.toggleUiHeader(true);
                    } else {
                        this.selModel.toggleUiHeader(false);
                    }
                }

                if (this.wasLocked) {
                    this.selModel.setLocked(true);
                }
            }
        }
    });


    //Script de la page Inventory
    IsSafQuantStockReached = function (value, meta) {
        var template1 = 'color:{0};';
        var res = "";
        if (value == false) { res = "No"; } else { res = "Yes"; }
        meta.style = Ext.String.format(template1, (value == false) ? "green" : "red");
        return res;
    };
    //Script de la page Inventory

    template = '<span style="color:{0};">{1}</span>';
    change = function (value) {
        return Ext.String.format(template, (value > 0) ? "green" : "red", value);
    };

    pctChange = function (value) {
        return Ext.String.format(template, (value > 0) ? "green" : "red", value + "%");
    };

    /*Début des CSS de la page RptReturnSaleController*/
    change_null = function (value) {
        if (value == null) {
            return Ext.String.format("Null");
        }
        else {
            return value;
        }
    };
    /*Fin des CSS de la page RptReturnSaleController*/

    /*Debut du Code JS de la page LensController*/
    SetLensCode = function () {

        App.ProductCode.clearValue();

        if (App.BifocalCode.getValue() != null) {
            App.ProductCode.setValue('DF');
        }

        if (App.IsProgressive.getValue() != true) {
            App.ProductCode.setValue('PROG');
        }
        //le reste du monde
        App.ProductCode.setValue(App.ProductCode.getValue() + ' ' +
                                 App.LensMaterialCode.getValue() + ' ' +
                                 App.LensColourCode.getValue() + ' ' +
                                 App.LensCoatingCode.getValue() + ' ' +
                                 App.LensNumberFullCode.getValue());

        if (App.BifocalCode.getValue() != null) {
            App.ProductCode.setValue(App.ProductCode.getValue() + ' ' + App.BifocalCode.getValue());
        }

        if (App.LensOtherCriterion.getValue() != null) {
            App.ProductCode.setValue(App.ProductCode.getValue() + ' ' + App.LensOtherCriterion.getValue());
        }
        return res;
    };

    /*Fin du Code JS de la page LensController*/

    ClosingDayStartedRenderer = function (value) {

        if (value == false) {
            return 'No';
        } else {
            return 'Yes';
        }

        return value;
    };


    BDStatutRenderer = function (value) {

        if (value == false) {
            return 'Close';
        } else {
            return 'Open';
        }

        return value;
    };

    prepareClassAccount = function (value, record) {
        return Ext.String.format(record.get('ClassAccount').ClassAccountNumber);
    };


    //edit = function (editor, e) {
    //    /*
    //        "e" is an edit event with the following properties:
    
    //            grid - The grid
    //            record - The record that was edited
    //            field - The field name that was edited
    //            value - The value being set
    //            originalValue - The original value for the field, before the edit.
    //            row - The grid table row
    //            column - The grid Column defining the column that was edited.
    //            rowIdx - The row index that was edited
    //            colIdx - The column index that was edited
    //    */

    //    // Call DirectMethod
    //    if (!(e.value === e.originalValue || (Ext.isDate(e.value) && Ext.Date.isEqual(e.value, e.originalValue)))) {
    //        Ext.net.DirectMethod.request({
    //            url: '@(Url.Action("Edit"))',
    //            params: {
    //                inventoryDirectoryLineID: e.record.data.InventoryDirectoryLineID,
    //                propertyName: e.field,
    //                oldValue: e.originalValue,
    //                newValue: e.value,
    //                inventoryDirectoryLine: e.record.data
    //            }
    //        });
    //    }
    //};

    //function stopRKey(evt) {
    //    var evt = (evt) ? evt : ((event) ? event : null);
    //    var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
    //    if ((evt.keyCode == 13) && (node.type == "text")) { return false; }
    //}
    //document.onkeypress = stopRKey;



};


function OnSliceAmountChanged() {

    var BuyType = App.BuyType.getValue();
    var SliceAmount = App.SliceAmount.getValue();

    if ((BuyType.indexOf("Credit") != -1) && (SliceAmount > 0)) {//Si le ùode de paiement est credit alors
        App.BuyType.setValue("TILL");
    }

    if ((SliceAmount <= 0)) {//Si le ùode de paiement est credit alors
        App.BuyType.setValue("Credit");
    }
}

/*Debut du code JS de la page OrderLensOrder*/
//Stop Form Submission of Enter Key Press
function stopRKey(evt) {
    var evt = (evt) ? evt : ((event) ? event : null);
    var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
    if ((evt.keyCode == 13) && (node.type == "text")) { return false; }
}

function ApplyLEToRE() {
    App.REAddition.setValue(App.LEAddition.getValue());
    App.REAxis.setValue(App.LEAxis.getValue());
    App.REIndex.setValue(App.LEIndex.getValue());
    App.RECylinder.setValue(App.LECylinder.getValue());
    App.RESphere.setValue(App.LESphere.getValue());
}

function ApplyREToLE() {
    App.LEAddition.setValue(App.REAddition.getValue());
    App.LEAxis.setValue(App.REAxis.getValue());
    App.LEIndex.setValue(App.REIndex.getValue());
    App.LECylinder.setValue(App.RECylinder.getValue());
    App.LESphere.setValue(App.RESphere.getValue());
}


function LEIndexChange() {

    //if (App.REIndex.getValue() == '') {

    var SupplyingName = App.SupplyingName.getValue();
    var LEIndex = ' ' + " INDEX " + App.LEIndex.getValue();
    if (App.LEIndex.getValue().length > 0) {
        if (SupplyingName.indexOf("INDEX") == -1) {//On n'a pas index dans la chaine
            SupplyingName = SupplyingName + LEIndex;
            App.SupplyingName.setValue(SupplyingName);
        }
        else {
            //Recherche et suppression de l'ancienne index
            var start = SupplyingName.indexOf("INDEX");
            var end = SupplyingName.length;
            var oldLEIndex = SupplyingName.slice(start, end);

            SupplyingName = SupplyingName.replace(oldLEIndex, "");
            //alert(SupplyingName);
            SupplyingName = SupplyingName.trim();

            SupplyingName = SupplyingName + LEIndex;
            App.SupplyingName.setValue(SupplyingName);
        }
    }

}

function REIndexChange() {

    //if (App.LEIndex.getValue() == '') {

    if (App.REIndex.getValue().length > 0) {

        var SupplyingName = App.SupplyingName.getValue();
        var REIndex = ' ' + " INDEX " + App.REIndex.getValue();

        if (SupplyingName.indexOf("INDEX") == -1) {//On n'a pas index dans la chaine
            SupplyingName = SupplyingName + REIndex;
            App.SupplyingName.setValue(SupplyingName);
        }
        else {
            //Recherche et suppression de l'ancienne index
            var start = SupplyingName.indexOf("INDEX");
            var end = SupplyingName.length;
            var oldLEIndex = SupplyingName.slice(start, end);
            SupplyingName = SupplyingName.replace(oldLEIndex, "");
            SupplyingName = SupplyingName.trim();

            SupplyingName = SupplyingName + REIndex;
            App.SupplyingName.setValue(SupplyingName);
        }
    }
}

/*Fin du code JS de la page OrderLensOrder*/

/*Début du code JS de la page PostedToSupplier*/

/* A header Checkbox of CheckboxSelectionModel deals with the current page only.
       This override demonstrates how to take into account all the pages.
       It works with local paging only. It is not going to work with remote paging.
    */






/*Fin du code JS de la page PostedToSupplier*/

/*Debut du code JS de la page ReceiveSpecialOrder*/

/* A header Checkbox of CheckboxSelectionModel deals with the current page only.
       This override demonstrates how to take into account all the pages.
       It works with local paging only. It is not going to work with remote paging.
    */


/*Fin du code JS de la page ReceiveSpecialOrder*/

/*Début des CSS de la page RptReturnSaleController*/
/*Fin des CSS de la page RptReturnSaleController*/

/*Debut des CSS de la page SaleController*/
//Stop Form Submission of Enter Key Press



function getDateTime() {
    var localTime = new Date();
    var year = localTime.getYear();
    var month = localTime.getMonth() + 1;
    var date = localTime.getDate();
    var hours = localTime.getHours();
    var minutes = localTime.getMinutes();
    var seconds = localTime.getSeconds();
    var heure = localTime.getHours() + ':' + localTime.getMinutes() + ':' + localTime.getSeconds();
    //document.getElementById("heureVente").setValue(heure);
    return heure;
    //at this point you can do with your results whatever you please
}








function updateErrorState(form) {
    var me = form,
        errorCmp, fields, errors;

    if (me.hasBeenDirty || me.getForm().isDirty()) { //prevents showing global error when form first loads
        errorCmp = me.down('#formErrorState');
        fields = me.getForm().getFields();
        errors = [];
        fields.each(function (field) {
            Ext.Array.forEach(field.getErrors(), function (error) {
                errors.push({ name: field.getFieldLabel(), error: error });
            });
        });
        setErrors(errorCmp, errors);
        me.hasBeenDirty = true;
    }
}

function boxLabelClick(e) {
    var target = e.getTarget('.terms'),
        win;

    e.preventDefault();

    if (target) {
        App.direct.ShowTerms();
    }
}

function setErrors(cmp, errors) {
    var me = cmp,
        baseCls = me.baseCls,
        tip = me.tooltips[0];

    errors = Ext.Array.from(errors);

    // Update CSS class and tooltip content
    if (errors.length) {
        me.addCls(baseCls + '-invalid');
        me.removeCls(baseCls + '-valid');
        me.update("Form has errors");
        tip.setDisabled(false);
        if (!tip.rendered) {
            tip.show();
        }
        tip.update(me.bin[0].apply(errors));
    } else {
        me.addCls(baseCls + '-valid');
        me.removeCls(baseCls + '-invalid');
        me.update("Form is valid");
        tip.setDisabled(true);
        tip.hide();
    }
}

function WareHouseManManagement() {
    if (App.AssigningToWareHouseMen.getValue() == true) {
        App.WareHouseManManagement.setHidden(false);

        App.AssigningDate.allowBlank = false;
        App.WareHouseMen.allowBlank = false;
    }

    if (App.AssigningToWareHouseMen.getValue() == false) {
        App.WareHouseManManagement.setHidden(true);

        App.AssigningDate.allowBlank = true;
        App.WareHouseMen.allowBlank = true;
    }

}

function activateValidateSupplierOrder() {
    if (App.InventoryDirectoryForm.isValid()) {
        App.FormAddInventoryDirectoryLine.setDisabled(false);
        App.InventoryDirectoryLines.setDisabled(false);

        if ((App.IsCadyEmpty.getValue() == 0)) {
            App.btnSave.setDisabled(false);
        } else {
            App.btnSave.setDisabled(true);
        }

    } else {
        App.FormAddInventoryDirectoryLine.setDisabled(true);
        App.InventoryDirectoryLines.setDisabled(true);
    }
}

function ActivateValidateCady() {
    if (App.FormAddInventoryDirectoryLine.isValid()) {
        App.AddToCady.setDisabled(false);
    }
    else {
        App.AddToCady.setDisabled(true);
    }
}


var filterTree = function (tf, e) {
    var tree = this.up("treepanel"),
        text = tf.getRawValue();

    tree.clearFilter();

    if (Ext.isEmpty(text, false)) {
        return;
    }

    if (e.getKey() === Ext.EventObject.ESC) {
        clearFilter();
    } else {
        var re = new RegExp(".*" + text + ".*", "i");

        tree.filterBy(function (node) {
            return re.test(node.data.text);
        });
    }
};

var clearFilter = function () {
    var field = this,
        tree = this.up("treepanel");

    field.setValue("");
    tree.clearFilter(true);
    tree.getView().focus();
};

var Description = function (id) {
    DESC.setValue(id);
};