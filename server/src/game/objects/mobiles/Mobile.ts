import WorldObject from "../WorldObject";

export default interface Mobile extends WorldObject{
	ctl: boolean;
	vel: number[];
	ang: number[];
}