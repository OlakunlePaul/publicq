$filePath = "C:\Users\hp\publicq\client\src\components\AssignmentExecution\AssignmentExecution.tsx"
$lines = Get-Content $filePath
$newLines = $lines[0..621] + $lines[728..($lines.Length-1)]
$newLines | Set-Content $filePath
