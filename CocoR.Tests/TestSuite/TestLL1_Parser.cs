
using System;



public class Parser {
	public const int _EOF = 0;
	public const int _a = 1;
	public const int _b = 2;
	public const int _c = 3;
	public const int _d = 4;
	public const int _e = 5;
	public const int _f = 6;
	public const int _g = 7;
	public const int _h = 8;
	public const int _i = 9;
	public const int maxT = 10;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void Test() {
		A();
		E();
		C();
		G();
		H();
		I();
		J();
	}

	void A() {
		if (la.kind == 1) {
			Get();
		} else if (la.kind == 1 || la.kind == 2 || la.kind == 3) {
			B();
		} else SynErr(11);
	}

	void E() {
		if (la.kind == 5 || la.kind == 6) {
			F();
		} else if (la.kind == 5) {
		} else SynErr(12);
		Expect(5);
	}

	void C() {
		while (la.kind == 1) {
			Get();
		}
		if (la.kind == 4) {
			D();
		}
		B();
	}

	void G() {
		if (la.kind == 1 || la.kind == 2) {
			if (eee) {
				if (la.kind == 1) {
					Get();
				} else if (la.kind == 2) {
					Get();
				} else SynErr(13);
			} else {
				Get();
			}
		}
		Expect(1);
	}

	void H() {
		if (la.kind == 1) {
			Get();
		}
		if (hhh) {
			Expect(1);
		}
		if (hhh) {
			Expect(1);
		}
		Expect(1);
	}

	void I() {
		while (la.kind == 1) {
			Get();
		}
		if (iii) {
			if (la.kind == 1) {
				Get();
			} else if (la.kind == 2) {
				Get();
			} else SynErr(14);
		} else if (la.kind == 2) {
			Get();
		} else SynErr(15);
	}

	void J() {
		while (aaa) {
			Expect(1);
		}
		while (la.kind == 1 || la.kind == 2) {
			if (eee) {
				if (la.kind == 1) {
					Get();
				} else if (la.kind == 2) {
					Get();
				} else SynErr(16);
			} else {
				Get();
			}
		}
		Expect(1);
	}

	void B() {
		while (la.kind == 2) {
			Get();
		}
		if (la.kind == 3) {
			Get();
		} else if (la.kind == 1) {
		} else SynErr(17);
		Expect(1);
	}

	void D() {
		Expect(4);
		if (la.kind == 2) {
			Get();
		}
	}

	void F() {
		if (la.kind == 6) {
			Get();
		}
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Test();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "a expected"; break;
			case 2: s = "b expected"; break;
			case 3: s = "c expected"; break;
			case 4: s = "d expected"; break;
			case 5: s = "e expected"; break;
			case 6: s = "f expected"; break;
			case 7: s = "g expected"; break;
			case 8: s = "h expected"; break;
			case 9: s = "i expected"; break;
			case 10: s = "??? expected"; break;
			case 11: s = "invalid A"; break;
			case 12: s = "invalid E"; break;
			case 13: s = "invalid G"; break;
			case 14: s = "invalid I"; break;
			case 15: s = "invalid I"; break;
			case 16: s = "invalid J"; break;
			case 17: s = "invalid B"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
