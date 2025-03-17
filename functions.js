
	function liveNeighboursCount(x, y) {
		return _test(x - 1, y - 1) + _test(x, y - 1) + _test(x + 1, y - 1) +
			_test(x - 1, y) + _test(x + 1, y) + _test(x - 1, y + 1) + _test(x, y + 1) + _test(x + 1, y + 1);
	}
