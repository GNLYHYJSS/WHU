#include<stdio.h>
#include<math.h>
void Q1(){			//�Ű���
	int i,j,m;
	for(i=1;i<9;i++)
	{
		m=9;
		for(j=1;j<=9;j++)
		{
			if(j>i)
				printf("%d",m);
			else printf("*");
			m--;
		}
		printf("\n");
	}
}

int Fib(int n){		//쳲���������
	if(n<1)
		return 0;
	if(n==1||n==2)
		return 1;
	else return Fib(n-1)+Fib(n-2);
}

void Q2(){
	int i,j,n,m;
	int flag;		//�ж��Ƿ�Ϊ����
	for(i=1;i<31;i++)	//���ǰ30������쳲�������
	{
		flag=1;
		n=Fib(i);
		m=(int)sqrt(n)+1;
		if(n<=1)		//1��������
			flag=0;
		else
		{
			for(j=2;j<m;j++)
			{
				if(n%j==0)
				{
					flag=0;
				}
			}
		}
		if(flag==1)
			printf("%d\n",n);
	}
}

void Q3(){
	int a[3]={2,3,5};	//a[0],a[1],a[2]�ֱ��ʾ���̻���ĸ���
	int i,j,k;
	for(i=0;i<=a[0];i++)
	{
		for(j=0;j<=a[1];j++)
		{
			for(k=0;k<=a[2];k++)
			{
				if(i+j+k==8)
					printf("����:%d ����:%d ����:%d\n",i,j,k);
			}
		}
	}
}

int divide(int n,int m){		//Q4�Ļ��ֵݹ�˼��
	if(n==1||m==1)
		return 1;
	if(n<m)
		return divide(n,n);
	if(n>m)
		return divide(n-m,m)+divide(n,m-1);
	if(n==m)
		return 1+divide(n,m-1);
}

void Q4(){	//6���������ֹ���11�������http://www.cnblogs.com/dongsheng/archive/2013/04/06/3002625.html
/*  6  
	5+1  
	4+2, 4+1+1  
	3+3, 3+2+1, 3+1+1+1  
	2+2+2, 2+2+1+1, 2+1+1+1+1  
	1+1+1+1+1+1+1*/
	int n;
	while(scanf("%d",&n)!=EOF)
	{
		printf("%d\n",divide(n,n));
	}
	
}

void Q5(){
	int i,j;
	int a[4]={0},b[4][8]={0};
	scanf("%d.%d.%d.%d",&a[0],&a[1],&a[2],&a[3]);
	for(i=0;i<4;i++){
		j=0;
		while(a[i]!=0)
		{
			b[i][j++]=a[i]%2;
			a[i]/=2;
		}
	}
	for(i=0;i<3;i++)
	{
		for(j=7;j>=0;j--)
			printf("%d",b[i][j]);
		printf(".");		
	}
	for(j=7;j>=0;j--)
		printf("%d",b[3][j]);
	printf("\n");
}

int main(){
	//Q1();		//�Ű���
	//Q2();		//쳲��������е���������
	//Q3();		//��ӡ����������
	Q4();		//�ֽ���ʾ����
	//Q5();		//ʮ����ipת��Ϊ��������ʾ

}