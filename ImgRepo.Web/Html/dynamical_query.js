class DynamicalQueryConstant {
    static operand = [
        { value: 0, text: '' },
        { value: 1, text: '且' },
        { value: 2, text: '或' },
    ]
    static operator = [
        { value: 0, text: '' },
        { value: 1, text: '等於' },
        { value: 2, text: '不等於' },
        { value: 3, text: '大於' },
        { value: 4, text: '大於或等於' },
        { value: 5, text: '小於' },
        { value: 6, text: '小於或等於' },
        { value: 7, text: '包含' },
        { value: 8, text: '屬於' },
    ]
    static FillOptions(optionArray, selectElement, className) {
        optionArray.forEach(item => {
            let option = document.createElement('option')
            option.value = item.value
            option.text = item.text
            option.classList.add(className);
            selectElement.appendChild(option)
        });
    }
    static FillNameOptions(nameOptionArray, selectElement, className) {
        if (!selectElement.classList.contains('dynamical-query-name-selection')) {
            selectElement.classList.add('dynamical-query-name-selection');
        }
        DynamicalQueryConstant.FillOptions(nameOptionArray, selectElement, className)
    }
    static FillOperandOptions(selectElement, className) {
        if (!selectElement.classList.contains('dynamical-query-operand-selection')) {
            selectElement.classList.add('dynamical-query-operand-selection');
        }
        DynamicalQueryConstant.FillOptions(DynamicalQueryConstant.operand, selectElement, className)
    }
    static FillOperatorOptions(selectElement, className) {
        if (!selectElement.classList.contains('dynamical-query-operator-selection')) {
            selectElement.classList.add('dynamical-query-operator-selection');
        }
        DynamicalQueryConstant.FillOptions(DynamicalQueryConstant.operator, selectElement, className)
    }
    static RemoveRow(elem) {
        $(elem).closest('tr').remove();
    }
    static AddRow(elem, template, nameOptionArray) {
        $(elem).closest('tr').after(template);
        let newRow = $(elem).closest('tr').next();
        DynamicalQueryConstant.FillNameOptions(nameOptionArray, newRow.find('.dynamical-query-name-selection').first()[0], 'text-center');
        DynamicalQueryConstant.FillOperatorOptions(newRow.find('.dynamical-query-operator-selection').first()[0], 'text-center');
        DynamicalQueryConstant.FillOperandOptions(newRow.find('.dynamical-query-operand-selection').first()[0], 'text-center');
        newRow.find('.dynamical-query-remove-button').first().click(function () {
            DynamicalQueryConstant.RemoveRow(this);
        });
        newRow.find('.dynamical-query-add-button').first().click(function () {
            DynamicalQueryConstant.AddRow(this, template, nameOptionArray);
        });
    }
    static GetDynamicQueryConditions(elem) {
        let conditions = [];
        let trs = $(elem).closest('tbody').find('tr');
        trs.each(function (index) {
            let item = trs[index];
            let name = $(item).find('.dynamical-query-name-selection').val();
            if (name == undefined) return;
            if (!name) {
                alert('請輸入條件名稱');
                return;
            }
            let operator = $(item).find('.dynamical-query-operator-selection').val();
            let operand = $(item).find('.dynamical-query-operand-selection').val();
            if (!operand) operand = 0;
            let constant = $(item).find('.dynamical-query-constant-input').val();
            if (constant == undefined) {
                alert('請輸入條件值');
                return;
            }
            conditions.push({ name: name, operator: Number(operator), operand: Number(operand), constant: constant });
        });
        return conditions;
    }
}
