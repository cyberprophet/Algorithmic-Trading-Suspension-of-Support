args <- commandArgs(TRUE)
filter <-
    c(
        ' ',
        '-',
        '"',
        '\\]',
        '[~!@#$%&*()_+=?<>,.����������"/\']',
        '\\[',
        '(��|��)',
        '[��-��]',
        '\\d{2,}',
        'URL',
        '������',
        'ipo',
        '�ܱ���',
        '���ɼ�',
        '��ǥ��',
        '��������',
        '��������',
        '�����',
        '������',
        '���ż�',
        '������',
        '���',
        '�������',
        '��������',
        '��κ�',
        '������',
        '������',
        '�����ÿ�����',
        'blog',
        '��밨',
        '�����',
        '��������',
        '�ð��Ѿ�',
        '��ȸ��',
        'etf',
        '����',
        '���������',
        'american',
        '�ǽð�',
        '�ڸ��ű�',
        '�����϶��',
        '������',
        '��',
        'https',
        'http',
        'www',
        '��¼�',
        '������',
        'kodex',
        '�ݿ���',
        '������',
        '������',
        'invest',
        'ceo',
        'com',
        '�ŷ����',
        '�����',
        '�̱�����',
        '�����',
        '�����ֽļ�������',
        '��ݱ�',
        '���Ѱ�',
        '���ͷ�',
        '������',
        '������',
        '�迭ȸ��',
        '������',
        '�����',
        '������',
        '������',
        '�ż���',
        '��ȸ��',
        '������',
        '�ξ�å',
        'ETF',
        'KODEX',
        'EPS',
        'INVEST',
        'IPO',
        'CEO',
        'BPS',
        'PBR',
        '��������',
        '���ȸ��',
        '�����ֽļ�',
        '�����ֽĺ���',
        'PER',
        '����μ�����ȸ��',
        'spac',
        '����',
        '����',
        '�Ż��',
        '��������',
        '���⵿��',
        '�ְ���',
        '�ֽ�ȸ��',
        '�ֿ���',
        '�߼�����',
        '�Ϲݱ�',
        'post',
        'POST',
        '�׵���',
        '�޵',
        '���̹�����',
        '��Ը�',
        '�̱�',
        '�����',
        '�����',
        '����',
        '������',
        '�츮����',
        '�ѱ��ŷ���',
        'PDF',
        'pdf',
        '�������ǰŷ���',
        '�ֽ�',
        '����',
        '������',
        '�Ұ���',
        '���������',
        '����̻�',
        '�����',
        '������',
        '���ŵ�',
        '����',
        '�����ϴ�',
        '�����',
        '�ֳ���',
        '�̻�ȸ',
        '�繫��ǥ',
        '�繫��',
        '�����ε���',
        'u947f',
        'u4e95',
        '����',
        '�ѵ���',
        '�ѱ���',
        "��",
        "��",
        "�Դϴ�",
        "�մϴ�",
        '�̤�',
        '����',
        'read',
        '�̳׿�',
        '��',
        '�̱���',
        '������',
        'KRX',
        'TIGER',
        'news',
        '����',
        '�ָ���'
    )
lapply(
    c(
        "rJava",
        "KoNLP",
        "multilinguer",
        "hash",
        "tau",
        "Sejong",
        "RSQLite",
        "devtools",
        "bit",
        "rex",
        "lazyeval",
        "htmlwidgets",
        "crosstalk",
        "promises",
        "later",
        "sessioninfo",
        "xopen",
        "bit64",
        "blob",
        "DBI",
        "memoise",
        "plogr",
        "covr",
        "DT",
        "rcmdcheck",
        "rversions",
        "wordcloud",
        "wordcloud2",
        "RColorBrewer",
        "tm",
        "stringr",
        "SnowballC",
        "webshot",
        "NIADic",
        "remotes"
    ),
    require,
    character.only = TRUE
)
useSejongDic()
codes <-
    read.csv(
        paste(args[2], '.csv', sep = '' , collapse = ''),
        sep = "\t",
        header =  FALSE,
        fileEncoding = 'UTF-8',
        stringsAsFactors = FALSE
    )
except <- data.frame(c('�ＺSDS', '2������', '5G', '4�����'), 'ncn')
mergeUserDic(codes)
mergeUserDic(except)
words <-
    unlist(sapply(readLines(
        paste(args[1], '.txt', sep = '', collapse = ''), encoding = 'UTF-8'
    ), extractNoun, USE.NAMES = F))
words <- Filter(function(x) {
    nchar(x) > 1
}, words)

for (i in 1:length(filter))
{
    words <- gsub(filter[i], '', words)
}
for (code in codes$V1)
{
    words <-
        gsub(paste(code, '\\S*', sep = '', collapse = ''), code, words)
}
for (ex in except[, 1])
{
    words <-
        gsub(paste(ex, '\\S*', sep = '', collapse = ''), ex, words)
}
options(mc.cores = 1)
tm.words <- VCorpus(VectorSource(words))
tm.words <- tm_map(tm.words, stripWhitespace)
tm.words <- tm_map(tm.words, removePunctuation)
matrix <-
    sort(rowSums(as.matrix(TermDocumentMatrix(tm.words))), decreasing = TRUE)
data <- data.frame(word = toupper(names(matrix)), freq = matrix)
write.csv(
    data[data$freq > 2, ],
    paste(args[1], '.csv', sep = '' , collapse = ''),
    row.names = FALSE,
    fileEncoding = 'UTF-8'
)
warnings()