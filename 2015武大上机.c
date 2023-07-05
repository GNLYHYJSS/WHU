#include<stdio.h>
#include<math.h>
void Q1(){			//排版题
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

int Fib(int n){		//斐波那契函数
	if(n<1)
		return 0;
	if(n==1||n==2)
		return 1;
	else return Fib(n-1)+Fib(n-2);
}

void Q2(){
	int i,j,n,m;
	int flag;		//判断是否为质数
	for(i=1;i<31;i++)	//输出前30的所有斐波那契数
	{
		flag=1;
		n=Fib(i);
		m=(int)sqrt(n)+1;
		if(n<=1)		//1不是质数
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
	int a[3]={2,3,5};	//a[0],a[1],a[2]分别表示红绿黄球的个数
	int i,j,k;
	for(i=0;i<=a[0];i++)
	{
		for(j=0;j<=a[1];j++)
		{
			for(k=0;k<=a[2];k++)
			{
				if(i+j+k==8)
					printf("红球:%d 绿球:%d 黄球:%d\n",i,j,k);
			}
		}
	}
}

int divide(int n,int m){		//Q4的划分递归思想
	if(n==1||m==1)
		return 1;
	if(n<m)
		return divide(n,n);
	if(n>m)
		return divide(n-m,m)+divide(n,m-1);
	if(n==m)
		return 1+divide(n,m-1);
}

void Q4(){	//6的整数划分共有11种详见：http://www.cnblogs.com/dongsheng/archive/2013/04/06/3002625.html
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
	//Q1();		//排版题
	//Q2();		//斐波那契数中的所有质数
	//Q3();		//打印所有球的组合
	Q4();		//分解显示数字
	//Q5();		//十进制ip转换为二进制显示

}