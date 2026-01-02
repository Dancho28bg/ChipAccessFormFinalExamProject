export function last7Days() {
    const to = new Date();
    const from = new Date();
    from.setDate(to.getDate() - 7);
    return { from, to };
}

export function last30Days() {
    const to = new Date();
    const from = new Date();
    from.setDate(to.getDate() - 30);
    return { from, to };
}

export function last3Months() {
    const to = new Date();
    const from = new Date();
    from.setMonth(to.getMonth() - 3);
    return { from, to };
}
