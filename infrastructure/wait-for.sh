#!/bin/sh

TIMEOUT=15
QUIET=0
WAIT_FOR_TIMEOUT=0
PROGRESS_BAR_WIDTH=50

echoerr() {
  if [ "$QUIET" -ne 1 ]; then printf "%s\n" "$*" 1>&2; fi
}

usage() {
  exitcode="$1"
  cat << USAGE >&2
Usage:
  $cmdname host:port [-t --quiet] [-t --timeout] [-h --help] [-- command args]
  -q | --quiet		Do not output any status messages
  -t | --timeout	Timeout in seconds, zero for no timeout
  -wft | --wait-for-timeout Should wait for timeout
  -h | --help		Help
  -- COMMAND ARGS	Execute command with args after the test finishes
USAGE
  exit "$exitcode"
}

preparebar() {
# $1 - bar length
# $2 - bar char
    barlen=$1
    barspaces=$(printf "%*s" "$1")
    barchars=$(printf "%*s" "$1" | tr ' ' "$2")
}

progressbar() {
# $1 - number (-1 for clearing the bar)
# $2 - max number
    if [ $1 -eq -1 ]; then
        printf "\r  $barspaces\r"
    else
        barch=$(($1*barlen/$2))
        barsp=$((barlen-barch))
        printf "\r[ ${barch}%% ][%.${barch}s%.${barsp}s]\r" "$barchars" "$barspaces"
    fi
}

wait_for() {
  
  preparebar 100 "#"
  
  if [ $# -lt 0 ]; then return; fi

  if [ $WAIT_FOR_TIMEOUT -eq 0 ] ; then
	echo "Waiting for connection..." >&2
  else
	echo "Waiting for timeout..." >&2  
  fi

  for i in `seq $TIMEOUT` ; do
    
	nc -z "$HOST" "$PORT" > /dev/null 2>&1
    result=$?	
	if [ $WAIT_FOR_TIMEOUT -eq 0 ]; then
		if [ $result -eq 0 ] ; then
		  exec "$@"
		  	progressbar 100 $TIMEOUT 
		  exit 0
		fi
	else
		if [ $result -gt 0 ] ; then
		  exec "$@"
		  	progressbar 100 $TIMEOUT 
		  exit 0
		fi
	fi
    sleep 1
	
	progressbar $i $TIMEOUT 
  done
  
  echo "\r"
  if [ $WAIT_FOR_TIMEOUT -eq 0 ] ; then
	echo "Operation timed out" >&2
  else
	echo "Operation timed out did not happed" >&2  
  fi
  
  exit 1
}

while [ $# -gt 0 ]
do
  argument=$2
  case "$1" in
    *:*)
		HOST=$(printf "%s\n" "$1"| cut -d : -f 1)
		PORT=$(printf "%s\n" "$1"| cut -d : -f 2)
		shift 1
		;;
    -wft | --wait-for-timeout)
		WAIT_FOR_TIMEOUT=1
		shift 1
		;;
	-t | --timeout)
		TIMEOUT="$argument"
		if [ "$TIMEOUT" = "" ]; then break; fi
		shift 2
		;;
    -q | --quiet)
		QUIET=1
		shift 1
		;;
    --) shift; break; ;;
    -h | --help) usage 0; ;;
    *) echoerr "Unknown argument: $1"; usage 1; ;;
  esac
done

if [ "$HOST" = "" -o "$PORT" = "" ]; then
  echoerr "Error: you need to provide a host and port to test."
  usage 2
fi

wait_for "$@"