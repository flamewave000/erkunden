export default interface PromiseResolver<T> {
	resolve: (value: T) => void;
	reject: (error: any) => void;
}