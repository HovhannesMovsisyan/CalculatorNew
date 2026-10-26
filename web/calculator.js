// Same logic as MainPage.xaml.cs in the MAUI app
const MaxDigits = 16;

let entry = "0";
let storedValue = null;
let pendingOperator = null;
let startNewEntry = true;

const resultEl = document.getElementById("result");
const expressionEl = document.getElementById("expression");

function onDigit(digit) {
    if (startNewEntry || entry === "0")
        entry = digit;
    else if (entry.replace(/\D/g, "").length < MaxDigits)
        entry += digit;

    startNewEntry = false;
    updateDisplay();
}

function onDecimal() {
    if (startNewEntry)
        entry = "0.";
    else if (!entry.includes("."))
        entry += ".";

    startNewEntry = false;
    updateDisplay();
}

function onOperator(operator) {
    // Chain calculations: 2 + 3 + evaluates the 2 + 3 before applying the next operator
    if (pendingOperator !== null && storedValue !== null && !startNewEntry) {
        const result = evaluate();
        if (result === null)
            return;
        entry = format(result);
    }

    storedValue = parseFloat(entry);
    pendingOperator = operator;
    startNewEntry = true;
    updateDisplay();
}

function onEquals() {
    if (pendingOperator === null || storedValue === null)
        return;

    const expression = `${format(storedValue)} ${pendingOperator} ${entry} =`;
    const result = evaluate();
    if (result === null)
        return;

    entry = format(result);
    storedValue = null;
    pendingOperator = null;
    startNewEntry = true;
    updateDisplay(expression);
}

function onClear() {
    reset();
    updateDisplay();
}

function onNegate() {
    if (entry === "0")
        return;

    entry = entry.startsWith("-") ? entry.slice(1) : "-" + entry;
    updateDisplay();
}

function onPercent() {
    entry = format(parseFloat(entry) / 100);
    startNewEntry = true;
    updateDisplay();
}

function onBackspace() {
    if (startNewEntry)
        return;

    entry = entry.length > 1 ? entry.slice(0, -1) : "0";
    if (entry === "-")
        entry = "0";
    updateDisplay();
}

// Returns null (and shows "Error") on divide by zero or overflow
function evaluate() {
    const left = storedValue;
    const right = parseFloat(entry);

    let result;
    switch (pendingOperator) {
        case "+": result = left + right; break;
        case "−": result = left - right; break;
        case "×": result = left * right; break;
        case "÷": result = right === 0 ? NaN : left / right; break;
        default: result = right;
    }

    if (!Number.isFinite(result)) {
        reset();
        resultEl.textContent = "Error";
        expressionEl.textContent = "";
        return null;
    }
    return result;
}

function reset() {
    entry = "0";
    storedValue = null;
    pendingOperator = null;
    startNewEntry = true;
}

function updateDisplay(expression) {
    resultEl.textContent = entry;
    expressionEl.textContent = expression
        ?? (pendingOperator !== null && storedValue !== null
            ? `${format(storedValue)} ${pendingOperator}`
            : "");
}

// Round off long fractions (e.g. 1 ÷ 3) and floating-point noise (0.1 + 0.2), drop trailing zeros
function format(value) {
    return String(Number(value.toFixed(10)));
}

// Button clicks
document.querySelector(".keys").addEventListener("click", e => {
    const button = e.target.closest("button");
    if (!button)
        return;

    const { digit, operator, action } = button.dataset;
    if (digit !== undefined) onDigit(digit);
    else if (operator !== undefined) onOperator(operator);
    else if (action === "decimal") onDecimal();
    else if (action === "equals") onEquals();
    else if (action === "clear") onClear();
    else if (action === "negate") onNegate();
    else if (action === "percent") onPercent();
    else if (action === "backspace") onBackspace();
});

// Keyboard support
const keyOperators = { "+": "+", "-": "−", "*": "×", "x": "×", "/": "÷" };

document.addEventListener("keydown", e => {
    if (e.ctrlKey || e.metaKey || e.altKey)
        return;

    const key = e.key;
    if (/^\d$/.test(key)) onDigit(key);
    else if (key === "." || key === ",") onDecimal();
    else if (key in keyOperators) onOperator(keyOperators[key]);
    else if (key === "Enter" || key === "=") onEquals();
    else if (key === "Backspace") onBackspace();
    else if (key === "Escape" || key === "Delete" || key.toLowerCase() === "c") onClear();
    else if (key === "%") onPercent();
    else
        return;

    e.preventDefault();
});
