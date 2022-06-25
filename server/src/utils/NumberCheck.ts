export function isInt(value: number): boolean {
	return value === parseInt(<any>value);
}

export function isFloat(value: number): boolean {
	return value === parseFloat(<any>value);
}