$path = $args[0]
$extract = ".\CLI\bin\Debug\net6.0\CLI.exe"

foreach ($a in (get-childitem -Path $path -Exclude .*))
{
    $r = & $extract $a | out-string
    echo "${a}" $r.Length
}