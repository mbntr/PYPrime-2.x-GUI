# cython: language_level=3, binding=True, wraparound=False, boundscheck=False, initializedcheck=False, unraisable_tracebacks=False, annotation_typing=False, cdivision=True

import sys
import math
import locale

from cython.view cimport array as cvarray
from libc.stdio cimport printf
from ctypes import WinDLL, wintypes, byref

locale.setlocale(locale.LC_ALL, 'en_US.UTF-8')

# Types
ctypedef unsigned long long ull

# Imports kernel32.dll
kernel32 = WinDLL('kernel32', use_last_error=True)

# Globals
cdef ull pr = 0
primes = []

prime = int(sys.argv[1])


cdef ull calc(unsigned char [::1] sieve, ull limit, ull sqrtlimit, ull qpf, ull start_time) nogil:
    cdef ull limit1, sqrtlimit1, loopstep, nextstep, x, x2, x2b3, x2b4, y, y2, n, m, o, nd, md
    cdef int loop = 0

    # Calculation
    limit1 = limit + 1
    sqrtlimit1 = sqrtlimit + 1

    loopstep = sqrtlimit / 10
    nextstep = loopstep

    # for x in range(1, sqrtlimit + 1):
    x = 1
    while x < sqrtlimit1:
        x2 = x ** 2
        x2b3 = x2 * 3
        x2b4 = x2b3 + x2

        # for y in range(1, sqrtlimit + 1):
        y = 1
        while y < sqrtlimit1:
            y2 = y ** 2

            n = x2b4 + y2
            nd = n % 12

            if n <= limit and (nd == 1 or nd == 5):
                sieve[n / 8] ^= 1 << (n % 8)

            m = x2b3 + y2
            md = m % 12

            if m <= limit and md == 7:
                sieve[m / 8] ^= 1 << (m % 8)

            o = x2b3 - y2

            if x > y and o <= limit and o % 12 == 11:
                sieve[o / 8] ^= 1 << (o % 8)

            y += 1

        if loop < 9 and x > nextstep:
            nextstep += loopstep
            loop += 1
    

        x += 1

    # for x in range(5, sqrtlimit):
    x = 5
    while x < sqrtlimit:
        if sieve[x]:
            x2 = x ** 2

            # for y in range(x2, limit + 1, x2):
            y = x2
            while y < limit1:
                sieve[y / 8] &= ~(1 << (y % 8));
                
                y += x2

        x += 1

    # for p in range(limit, 5, -1):
    x = limit // 8
    while x > 0:
        if sieve[x] == 0:
            x -= 1
            continue

        break

    return x

cdef benchmark(ull limit, ull qpf):
    
    start_time = wintypes.LARGE_INTEGER()
    end_time = wintypes.LARGE_INTEGER()


    cdef ull resultx, result
    cdef ull sieve_len = (limit // 8) + 1
    # print_memalloc(sieve_len)
        
    sieve_data = cvarray(shape=(sieve_len,), itemsize=sizeof(unsigned char), format="B")
    cdef unsigned char[::1] sieve = sieve_data

    
    # Start timestamp
    kernel32.QueryPerformanceCounter(byref(start_time))

    # Calculation
    resultx = calc(sieve, limit, int(math.sqrt(limit)), qpf, start_time.value)

    # End timestamp
    kernel32.QueryPerformanceCounter(byref(end_time))

    # Finish the calculation
    result = (sieve[resultx].bit_length() - 1) + resultx * 8
    time = round((end_time.value - start_time.value) / qpf, 3)


    return [ result, time ]
    
qpf = wintypes.LARGE_INTEGER()
kernel32.QueryPerformanceFrequency(byref(qpf))

print(benchmark(prime, qpf.value)[0], benchmark(prime, qpf.value)[1])
   