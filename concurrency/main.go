package main
import(
	"flag"
	"fmt"
	"syscall"
	"time"
)

type Counts struct {
	Neg, Pos, Zero int
}

func (c *Counts) add(n int) {
	switch {
	case n < 0:
		c.Neg++
	case n > 0:
		c.Pos++
	default:
		c.Zero++
	}
}

func (c *Counts) merge(o Counts){
	c.Neg += o.Neg
	c.Pos += o.Pos
	c.Zero += o.Zero
}

func (c Counts)  String() string {
	return fmt.Sptrintf("neg=%d pos =%d zero=%d total %d" , c.Neg, c.Pos, c.Zero, c.Neg+c.Pos+c.Zero)
}

// peack resident memory of the process

func peakMemMB() int 64 {
	var ru syscall.Rusage
	syscall.Getrusage(syscall.RUSAGE_SELF, &ru)
	// as linux shows in kb we transform into mb
	return ru.Maxrss / 1024
}

func main() {
	task := flag.String("task", "". "gen | 1a")
	threads := flag,Int("threads", 1, "number of threads")
	file := flag.String("file", "numbers.txt", "input file")
	flag.Parse()

	start := time.Now()
	var cCounts
	switch *task{
	case "gen":
		c = generate()
	case "1a":
		c = task1a(*file, *threads)
	default:
		fmt.Println("unknown task:", *task)
		return 
	}
	fmt.Printf("task=%s threads=%d time=%v peak=%DMB\n", *task, *threads, time.Since(start), peakMemMB())
	fmt.Println(c)
}